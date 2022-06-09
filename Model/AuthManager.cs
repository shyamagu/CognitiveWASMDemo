namespace BlazorCognitiveWASM.Model;

public class AuthManager{

    //Face API
    public string FaceApi_Key {get;set;} = String.Empty;

    public string FaceApi_Endpoint {get;set;} = String.Empty;

    public void SetFaceApiAuthentication(AuthModel authModel){
        
        FaceApi_Key = authModel.Key;
        FaceApi_Endpoint = authModel.Endpoint;
    }

    public Boolean IsFaceApiAuthAlreadySet => (!String.IsNullOrEmpty(FaceApi_Key) && !String.IsNullOrEmpty(FaceApi_Endpoint));

    //READ API
    public string ReadApi_Key {get;set;} = String.Empty;

    public string ReadApi_Endpoint {get;set;} = String.Empty;

    public void SetReadApiAuthentication(AuthModel authModel){
        
        ReadApi_Key = authModel.Key;
        ReadApi_Endpoint = authModel.Endpoint;
    }

    public Boolean IsReadApiAuthAlreadySet => (!String.IsNullOrEmpty(ReadApi_Key) && !String.IsNullOrEmpty(ReadApi_Endpoint));


}