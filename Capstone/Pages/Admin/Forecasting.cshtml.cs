using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.IO;

namespace Capstone.Pages.Admin
{
    public class ForecastingModel : PageModel
    {
        // Define separate classes for each forecast output
        public class PaxQtyForecast
        {
            public float[] ForecastedPaxQty { get; set; }
        }

        public class PaxAmountForecast
        {
            public float[] ForecastedPaxAmount { get; set; }
        }

        public (List<float> ForecastedPaxQtyValues, List<float> ForecastedPaxAmountValues) ForecastedValues { get; private set; }
        public List<ForecastedAttendanceWithDate> ForecastedResults { get; private set; }

        public void OnGet()
        {
            // Populate ForecastedValues using PerformForecasting()
            ForecastedResults = PerformForecasting();
        }

        public List<ForecastedAttendanceWithDate> PerformForecasting()
        {
            var mlContext = new MLContext();

            // Load data
            IDataView dataView = mlContext.Data.LoadFromTextFile<AttendanceData>(
                "C:\\Users\\Tine Cacapit\\source\\repos\\WiibandSystem1\\Capstone\\Dataset.csv",
                hasHeader: true,
                separatorChar: ',');

            // Set parameters for monthly forecasting
            int windowSize = 3; // Looking back 3 months for trends
            int seriesLength = 12; // Monthly data over a year
            int trainSize = 36; // Training over 3 years of monthly data
            int horizon = 12; // Forecasting the next 12 months

            // Forecasting pipeline for OverallPaxQty
            var paxQtyPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedPaxQty",
                inputColumnName: nameof(AttendanceData.OverallPaxQty),
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: trainSize,
                horizon: horizon);

            // Train the model for OverallPaxQty
            var paxQtyModel = paxQtyPipeline.Fit(dataView);

            // Make predictions for OverallPaxQty
            IDataView paxQtyPredictions = paxQtyModel.Transform(dataView);
            var forecastedPaxQtyData = mlContext.Data.CreateEnumerable<PaxQtyForecast>(paxQtyPredictions, reuseRowObject: false);

            // Forecasting pipeline for OverallPaxAmount
            var paxAmountPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedPaxAmount",
                inputColumnName: nameof(AttendanceData.OverallPaxAmount),
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: trainSize,
                horizon: horizon);

            // Train the model for OverallPaxAmount
            var paxAmountModel = paxAmountPipeline.Fit(dataView);

            // Make predictions for OverallPaxAmount
            IDataView paxAmountPredictions = paxAmountModel.Transform(dataView);
            var forecastedPaxAmountData = mlContext.Data.CreateEnumerable<PaxAmountForecast>(paxAmountPredictions, reuseRowObject: false);

            // Collect forecasted values with corresponding dates
            var forecastedResults = new List<ForecastedAttendanceWithDate>();
            var lastDate = mlContext.Data.CreateEnumerable<AttendanceData>(dataView, reuseRowObject: false).Last().Date;

            for (int i = 0; i < horizon; i++)
            {
                var forecastedPaxQty = forecastedPaxQtyData.First().ForecastedPaxQty[i];
                var forecastedPaxAmount = forecastedPaxAmountData.First().ForecastedPaxAmount[i];

                // Incrementing date by month for each forecasted value
                var forecastDate = lastDate.AddMonths(i + 1);

                forecastedResults.Add(new ForecastedAttendanceWithDate
                {
                    ForecastDate = forecastDate,
                    ForecastedPaxQty = forecastedPaxQty,
                    ForecastedPaxAmount = forecastedPaxAmount
                });
            }

            return forecastedResults;
        }

        public class ForecastedAttendanceWithDate
        {
            public DateTime ForecastDate { get; set; }
            public float ForecastedPaxQty { get; set; }
            public float ForecastedPaxAmount { get; set; }
        }

        // Define the data structure for input data
        public class AttendanceData
        {
            [LoadColumn(0)]
            public DateTime Date { get; set; }

            [LoadColumn(2)]
            public float GeneralAdmission { get; set; }

            [LoadColumn(3)]
            public float ExtendedHour { get; set; }

            [LoadColumn(4)]
            public float PWDGA { get; set; }

            [LoadColumn(5)]
            public float EarlyJump { get; set; }

            [LoadColumn(6)]
            public float OverallPaxQty { get; set; }

            [LoadColumn(7)]
            public float OverallPaxAmount { get; set; }

            [LoadColumn(9)]
            public float TenHoursMultipass { get; set; }

            [LoadColumn(10)]
            public float TwentyHoursMultipass { get; set; }
        }
    }
}
