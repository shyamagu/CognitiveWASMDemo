namespace BlazorCognitiveWASM.Model.Read;
public class ReadResultModel{

    public string ImageDataUrl = String.Empty;

    public List<string> ReadAnalysisResult = new();

    public ReadResultModel(){
    }
    
    public ReadResultModel(string imageDataUrl, List<string> readAnalysisResult){
        ImageDataUrl = imageDataUrl;
        ReadAnalysisResult= readAnalysisResult;
    }
}