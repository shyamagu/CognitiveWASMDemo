using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
namespace BlazorCognitiveWASM.Model.Face;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

public class FaceService {

    const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;

    public List<FaceResultModel> FaceResult = new();

    public IFaceClient GetFaceClient(string key,string endpoint){
        return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
    }

    public async Task<IList<DetectedFace>> DetectFaceExtract(IFaceClient client, Stream imageStream){
        return await client.Face.DetectWithStreamAsync(imageStream,
            returnFaceLandmarks: true,
            returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
            FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
            FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
            FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile},
            // FaceAttributeType.QualityForRecognition is missing
            // We specify detection model 1 because we are retrieving attributes.
            detectionModel: DetectionModel.Detection01,
            recognitionModel: RECOGNITION_MODEL4);
    }

    public List<string> GetFaceAnalysisResult(DetectedFace face){

        List<string> faceApiResult = new();

        faceApiResult.Add($"Age : {face.FaceAttributes.Age}");
        faceApiResult.Add($"Blur : {face.FaceAttributes.Blur.BlurLevel}");
        
        string emotionType = string.Empty;
        double emotionValue = 0.0;
        Emotion emotion = face.FaceAttributes.Emotion;
        if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
        if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
        if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
        if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
        if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
        if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
        if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
        if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
        faceApiResult.Add($"Main Emotion : {emotionType}");

        faceApiResult.Add(
            $"HeadPose : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}"
        );

        faceApiResult.Add($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
        faceApiResult.Add($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
        faceApiResult.Add($"Glasses : {face.FaceAttributes.Glasses}");

        // Get hair color
        Hair hair = face.FaceAttributes.Hair;
        string? color = null;
        if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
        HairColorType returnColor = HairColorType.Unknown;
        double maxConfidence = 0.0f;
        foreach (HairColor hairColor in hair.HairColor)
        {
            if (hairColor.Confidence <= maxConfidence) { continue; }
            maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
        }
        faceApiResult.Add($"Hair : {color}");
        faceApiResult.Add($"Bald : {face.FaceAttributes.Hair.Bald}");
        faceApiResult.Add($"Invisible : {face.FaceAttributes.Hair.Invisible}");

        // Get accessories of the faces
        List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
        int count = face.FaceAttributes.Accessories.Count;
        string accessory; string[] accessoryArray = new string[count];
        if (count == 0) { accessory = "NoAccessories"; }
        else
        {
            for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
            accessory = string.Join(",", accessoryArray);
        }
        faceApiResult.Add($"Accessories : {accessory}");

        // Get more attributes
        faceApiResult.Add($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
        faceApiResult.Add($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
        faceApiResult.Add($"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
            $" {string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
        faceApiResult.Add($"Smile : {face.FaceAttributes.Smile}");

        return faceApiResult;
    }

    public string GetFaceApiResultImageUrl(DetectedFace face,byte[] buffer){

        var landmarks = face.FaceLandmarks;
        var rectangle = face.FaceRectangle;

        Image image = Image.Load(buffer);
        //ここでランドマークの値をつかって、画像をごにょごにょして、imageDataUrlにながしこめばよい
        if(landmarks is not null){
            foreach (var prop in landmarks.GetType().GetProperties())
            {
                if(prop.GetValue(landmarks) is Coordinate point){
                    EllipsePolygon pointCircle = new EllipsePolygon(
                        x:(float) point.X,
                        y:(float) point.Y,
                        radius:2.0f);
                    image.Mutate(x=>x.Fill(Color.Red, pointCircle));
                }
            }
        }

        if(rectangle is not null){
            RectangularPolygon rectPolygon = new RectangularPolygon(rectangle.Left,rectangle.Top,rectangle.Width,rectangle.Height);
            var pen = Pens.Solid(Color.Red,1.0f);
            image.Mutate(ctx=>ctx.Draw(pen,rectPolygon));
        }

        return image.ToBase64String(JpegFormat.Instance);
    }
}