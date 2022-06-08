namespace BlazorCognitiveWASM.Model;

public class AuthManager{
    public string FaceApi_Key {get;set;} = String.Empty;

    public string FaceApi_Endpoint {get;set;} = String.Empty;

    public void SetFaceApiAuthentication(AuthModel authModel){
        
        FaceApi_Key = authModel.Key;
        FaceApi_Endpoint = authModel.Endpoint;
    }

    public Boolean IsFaceApiAuthAlreadySet => (!String.IsNullOrEmpty(FaceApi_Key) && !String.IsNullOrEmpty(FaceApi_Endpoint));
}