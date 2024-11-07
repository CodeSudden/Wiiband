using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone.Pages.Admin
{
    public class ForecastingModel : PageModel
    {
        public string ErrorMessage { get; private set; } // Error message to display in the view if needed
        public List<ForecastedAttendanceWithDate> ForecastedResults { get; private set; }

        public class SalesPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool HighSalesPrediction { get; set; }
        }

        public class PaxQtyForecast
        {
            public float[] ForecastedPaxQty { get; set; }
        }

        public class PaxAmountForecast
        {
            public float[] ForecastedPaxAmount { get; set; }
        }

        public void OnGet()
        {
            try
            {
                // Perform forecasting with seasonality and error handling
                ForecastedResults = PerformForecastingWithSeasonality();

                // Retrieve historical data to train the logistic regression model
                var mlContext = new MLContext();
                IDataView dataView = mlContext.Data.LoadFromTextFile<AttendanceData>(
                    "C:\\Users\\Tine Cacapit\\source\\repos\\WiibandSystem1\\Capstone\\Dataset.csv",
                    hasHeader: true,
                    separatorChar: ',');

                var historicalData = mlContext.Data.CreateEnumerable<AttendanceData>(dataView, reuseRowObject: false).ToList();

                // Train the logistic regression model
                var salesPredictorModel = TrainLogisticRegressionModel(historicalData);

                // Apply logistic regression predictions to each forecasted result
                foreach (var result in ForecastedResults)
                {
                    var inputData = new AttendanceData
                    {
                        Date = result.ForecastDate,
                        OverallPaxQty = result.ForecastedPaxQty,
                        OverallPaxAmount = result.ForecastedPaxAmount
                    };

                    // Predict if the sales are high or low
                    result.HighSalesPrediction = PredictHighSales(inputData, salesPredictorModel);
                }
            }
            catch (Exception ex)
            {
                // Capture any errors that occur and display to the user
                ErrorMessage = $"An error occurred while generating the forecast: {ex.Message}";
                Console.WriteLine(ex); // For development, use logging in production
            }
        }

        public List<ForecastedAttendanceWithDate> PerformForecastingWithSeasonality()
        {
            var mlContext = new MLContext();
            var forecastedResults = new List<ForecastedAttendanceWithDate>();

            // Load data with error handling
            IDataView dataView;
            try
            {
                dataView = mlContext.Data.LoadFromTextFile<AttendanceData>(
                    "C:\\Users\\Tine Cacapit\\source\\repos\\WiibandSystem1\\Capstone\\Dataset.csv",
                    hasHeader: true,
                    separatorChar: ',');
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading data. Please check the dataset file and ensure it's accessible. Details: " + ex.Message);
            }

            // Check for missing values in critical columns
            var dataEnumerable = mlContext.Data.CreateEnumerable<AttendanceData>(dataView, reuseRowObject: false).ToList();

            if (dataEnumerable.Any(row => row.OverallPaxQty == 0 || row.OverallPaxAmount == 0 || row.Month == 0 || row.Year == 0))
            {
                throw new Exception("Dataset contains missing values in required fields. Ensure 'Overall_Pax_QTY', 'Overall_Pax_Amount', 'Month', and 'Year' have no missing values.");
            }

            // Set parameters to capture monthly seasonality
            int windowSize = 12; // Capture a yearly seasonality
            int seriesLength = 36; // 3 years for better trend recognition
            int trainSize = 36;
            int horizon = 12;

            try
            {
                // Forecasting pipeline for OverallPaxQty
                var paxQtyPipeline = mlContext.Transforms.Concatenate("Features", "Month", "Year")
                    .Append(mlContext.Forecasting.ForecastBySsa(
                        outputColumnName: "ForecastedPaxQty",
                        inputColumnName: nameof(AttendanceData.OverallPaxQty),
                        windowSize: windowSize,
                        seriesLength: seriesLength,
                        trainSize: trainSize,
                        horizon: horizon));

                var paxQtyModel = paxQtyPipeline.Fit(dataView);
                IDataView paxQtyPredictions = paxQtyModel.Transform(dataView);
                var forecastedPaxQtyData = mlContext.Data.CreateEnumerable<PaxQtyForecast>(paxQtyPredictions, reuseRowObject: false);

                // Forecasting pipeline for OverallPaxAmount
                var paxAmountPipeline = mlContext.Transforms.Concatenate("Features", "Month", "Year")
                    .Append(mlContext.Forecasting.ForecastBySsa(
                        outputColumnName: "ForecastedPaxAmount",
                        inputColumnName: nameof(AttendanceData.OverallPaxAmount),
                        windowSize: windowSize,
                        seriesLength: seriesLength,
                        trainSize: trainSize,
                        horizon: horizon));

                var paxAmountModel = paxAmountPipeline.Fit(dataView);
                IDataView paxAmountPredictions = paxAmountModel.Transform(dataView);
                var forecastedPaxAmountData = mlContext.Data.CreateEnumerable<PaxAmountForecast>(paxAmountPredictions, reuseRowObject: false);

                var lastDate = dataEnumerable.Last().Date;

                for (int i = 0; i < horizon; i++)
                {
                    var forecastedPaxQty = forecastedPaxQtyData.First().ForecastedPaxQty[i];
                    var forecastedPaxAmount = forecastedPaxAmountData.First().ForecastedPaxAmount[i];
                    var forecastDate = lastDate.AddMonths(i + 1);

                    forecastedResults.Add(new ForecastedAttendanceWithDate
                    {
                        ForecastDate = forecastDate,
                        ForecastedPaxQty = forecastedPaxQty,
                        ForecastedPaxAmount = forecastedPaxAmount
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during forecasting. Please ensure the dataset and model parameters are set correctly. Details: " + ex.Message);
            }

            return forecastedResults;
        }

        private ITransformer TrainLogisticRegressionModel(List<AttendanceData> trainingData)
        {
            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms.Concatenate("Features", "OverallPaxQty", "OverallPaxAmount")
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(AttendanceData.HighSales)))
                .Append(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression());

            return pipeline.Fit(data);
        }

        public bool PredictHighSales(AttendanceData input, ITransformer model)
        {
            var mlContext = new MLContext();
            var predictionEngine = mlContext.Model.CreatePredictionEngine<AttendanceData, SalesPrediction>(model);
            var prediction = predictionEngine.Predict(input);
            return prediction.HighSalesPrediction;
        }

        public class ForecastedAttendanceWithDate
        {
            public DateTime ForecastDate { get; set; }
            public float ForecastedPaxQty { get; set; }
            public float ForecastedPaxAmount { get; set; }
            public bool HighSalesPrediction { get; set; }
        }

        public class AttendanceData
        {
            public int Id { get; set; }

            [LoadColumn(0)]
            public DateTime Date { get; set; }

            [LoadColumn(6)]
            public float OverallPaxQty { get; set; }

            [LoadColumn(7)]
            public float OverallPaxAmount { get; set; }

            [LoadColumn(10)]
            public int Year { get; set; }

            [LoadColumn(11)]
            public int Month { get; set; }

            public bool HighSales => OverallPaxAmount > 10000; // Threshold for high sales
        }
    }
}
