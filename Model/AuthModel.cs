public class AuthModel{
    public string Key {get;set;} = String.Empty;

    public string Endpoint {get;set;} = String.Empty;

    public AuthModel(){
    }
    public AuthModel(string key, string endpoint){
        Key = key;
        Endpoint = endpoint;
    }
}