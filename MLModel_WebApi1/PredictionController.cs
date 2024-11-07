using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PredictionController : ControllerBase
{
    [HttpPost]
    public ActionResult<ModelOutput> Predict([FromBody] ModelInput input)
    {
        var result = ConsumeModel.Predict(input);
        return Ok(result);
    }
}
