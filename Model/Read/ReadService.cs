using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BlazorCognitiveWASM.Model.Read;

public class ReadService{

    public List<ReadResultModel> ReadResult = new();
    public ComputerVisionClient GetReadClient(string key,string endpoint){
        return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)){ Endpoint = endpoint };
    }

    public async Task<string> GetOperationLocation(ComputerVisionClient client, Stream imageStream){
        var textHeaders = await client.ReadInStreamAsync(imageStream);
        return textHeaders.OperationLocation;
    }
    private const int numberOfCharsInOperationId = 36;

    public async Task<ReadOperationResult> GetReadOperationResult(ComputerVisionClient client,string operationLocation){
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        ReadOperationResult results;
        do{
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

        return results;
    }

    public List<string> GetReadAnalysisResult(ReadOperationResult readOperationResult){
        List<string> readApiResult = new();

        var textFileResult = readOperationResult.AnalyzeResult.ReadResults;
        foreach(ReadResult page in textFileResult){
            foreach(Line line in page.Lines){
                readApiResult.Add(string.Format("{0} ({1})",line.Text,line.Appearance.Style.Name));
            }
        }

        return readApiResult;
    }

    public string GetReadApiResultImageUrl(ReadOperationResult readOperationResult,byte[] buffer){

        SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(buffer);

        var textFileResult = readOperationResult.AnalyzeResult.ReadResults;

        foreach(ReadResult page in textFileResult){
            foreach(Line line in page.Lines){
                var points = new PointF[5];
                points[0] = new PointF(x:(float)line.BoundingBox[0]!,y:(float)line.BoundingBox[1]!);
                points[1] = new PointF(x:(float)line.BoundingBox[2]!,y:(float)line.BoundingBox[3]!);
                points[2] = new PointF(x:(float)line.BoundingBox[4]!,y:(float)line.BoundingBox[5]!);
                points[3] = new PointF(x:(float)line.BoundingBox[6]!,y:(float)line.BoundingBox[7]!);
                points[4] = new PointF(x:(float)line.BoundingBox[0]!,y:(float)line.BoundingBox[1]!);

                var pen = Pens.Solid(Color.Red,1.0f);
                image.Mutate(ctx=>ctx.DrawLines(pen,points));                
            }
        }

        return image.ToBase64String(JpegFormat.Instance);

    }
}