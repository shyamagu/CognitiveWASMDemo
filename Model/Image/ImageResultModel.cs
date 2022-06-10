namespace BlazorCognitiveWASM.Model.Image;
public class ImageResultModel{

    public string ImageDataUrl = String.Empty;

    public List<string> ImageAnalysisResult = new();

    public ImageResultModel(){
    }
    
    public ImageResultModel(string imageDataUrl, List<string> imageAnalysisResult){
        ImageDataUrl = imageDataUrl;
        ImageAnalysisResult= imageAnalysisResult;
    }
}