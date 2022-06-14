using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using SixLabors.ImageSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BlazorCognitiveWASM.Model.Image;

public class ImageService{
    public List<ImageResultModel> ImageResult = new();

    private readonly HttpClient httpClient;
    public ImageService(HttpClient httpClient){
        this.httpClient = httpClient;
    }

    public ComputerVisionClient GetImageClient(string key,string endpoint){
        return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)){ Endpoint = endpoint };
    }

    private FontCollection fontCollection = new FontCollection();
    private FontFamily fontFamily;

    private Font? font = null;
    public async Task setFontFamily(){

            if(font is null){
                var fontBytes = await httpClient.GetByteArrayAsync("/font/BebasNeue-Regular.ttf");
                fontCollection.Add(new MemoryStream(fontBytes));
                fontFamily = fontCollection.Get("Bebas Neue");
                font = new Font(fontFamily,24f,FontStyle.Regular);
            }
    }

    public static async Task<ImageAnalysis> GetAnalyzeImageResult(ComputerVisionClient client, Stream stream){


        List<VisualFeatureTypes?> visualFeatures = new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Categories, 
            VisualFeatureTypes.Description,  
            VisualFeatureTypes.Faces, 
            VisualFeatureTypes.Objects,
            VisualFeatureTypes.Tags
        };

        List<Details?> details = new List<Details?>();

        ImageAnalysis results = await client.AnalyzeImageInStreamAsync(stream, visualFeatures);

        return results;
    }

    public List<string> GetImageAnalysisResult(ImageAnalysis results){
        List<string> imageApiResult = new();

        foreach(var caption in results.Description.Captions){
            imageApiResult.Add(caption.Text);
        }

        foreach(var category in results.Categories){
            imageApiResult.Add(string.Format(" {0},({1})",category.Name,category.Score));
        }

        return imageApiResult;
    }

    public string GetImageApiResultImageUrl(ImageAnalysis results,byte[] buffer){

        SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(buffer);

        foreach(var face in results.Faces){
            TextOptions options = new(font!){
                    Origin = new PointF(face.FaceRectangle.Left-20,face.FaceRectangle.Top-10)
                };
            IBrush brush;
            IPen penAge = Pens.Solid(Color.White,1);;
            if(face.Gender==Gender.Male){
                brush = Brushes.Solid(Color.Blue);
                image.Mutate(ctx=>ctx.DrawText(options,face.Age.ToString(),brush,penAge));
            }else{
                brush = Brushes.Solid(Color.Red);
                image.Mutate(ctx=>ctx.DrawText(options,face.Age.ToString(),brush,penAge));
            }
        }

        foreach(var obj in results.Objects){
            RectangularPolygon rectPolygon = new RectangularPolygon(obj.Rectangle.X,obj.Rectangle.Y,obj.Rectangle.W,obj.Rectangle.H);
            var pen = Pens.Solid(Color.Red,1.0f);
            image.Mutate(ctx=>ctx.Draw(pen,rectPolygon));

            TextOptions options = new(font!){
                    Origin = new PointF(obj.Rectangle.X,obj.Rectangle.Y-20)
                };
            IPen penAge = Pens.Solid(Color.White,1);
            IBrush brush = Brushes.Solid(Color.Black);
            image.Mutate(ctx=>ctx.DrawText(options,obj.ObjectProperty.ToString(),brush,penAge));

//            imageApiResult.Add(string.Format("{0},{1}",obj.ObjectProperty.ToString(),obj.Confidence));
        }

        return image.ToBase64String(JpegFormat.Instance);
    }

}