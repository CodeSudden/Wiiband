using Microsoft.ML;
using System;

public static class ConsumeModel
{
    private static Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictionEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(CreatePredictionEngine);

    public static ModelOutput Predict(ModelInput input)
    {
        return PredictionEngine.Value.Predict(input);
    }

    private static PredictionEngine<ModelInput, ModelOutput> CreatePredictionEngine()
    {
        var mlContext = new MLContext();
        ITransformer mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);
        return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
    }
}
