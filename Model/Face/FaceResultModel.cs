using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

public class FaceResultModel{

    public string ImageDataUrl = String.Empty;

    public List<string> FaceAnalysisResult = new();

    public FaceResultModel(){
    }
    
    public FaceResultModel(string imageDataUrl, List<string> faceAnalysisResult){
        ImageDataUrl = imageDataUrl;
        FaceAnalysisResult= faceAnalysisResult;
    }

}