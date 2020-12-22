using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace SimpleImageGalerry.Controllers
{
  public   class FaceRe
    {
        static string sourcePersonGroup = null;
        static string targetPersonGroup = null;

        IFaceClient client;

        string SUBSCRIPTION_KEY = "bf09f0841186495e9ede0919cf7f82b3";
        string ENDPOINT = "https://eastus.api.cognitive.microsoft.com/";

        string TARGET_SUBSCRIPTION_KEY = "bf09f0841186495e9ede0919cf7f82b3";
        string TARGET_ENDPOINT = "https://eastus.api.cognitive.microsoft.com/";

        public string recognitionModel;

        const string IMAGE_BASE_URL = @"C:\Users\RomanKoltun\Desktop\SimpleImageGalerry\SimpleImageGalerry\wwwroot\image\";
        const string IMAGE_BASE_URL11 = @"C:\Users\RomanKoltun\Desktop\SimpleImageGalerry\SimpleImageGalerry\wwwroot\image\";
        public FaceRe()
        {

     
            Guid AZURE_SUBSCRIPTION_ID = new Guid("bf09f0841186495e9ede0919cf7f82b3");

            Guid TARGET_AZURE_SUBSCRIPTION_ID = new Guid("bf09f0841186495e9ede0919cf7f82b3");
            const string RECOGNITION_MODEL2 = RecognitionModel.Recognition01;
            const string RECOGNITION_MODEL1 = RecognitionModel.Recognition02;

            client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);


            IFaceClient clientTarget = Authenticate(TARGET_ENDPOINT, TARGET_SUBSCRIPTION_KEY);

            DetectFaceExtract(client, IMAGE_BASE_URL, RECOGNITION_MODEL2).Wait();

            //IdentifyInPersonGroup(client, IMAGE_BASE_URL, RECOGNITION_MODEL1).Wait();

            recognitionModel = RECOGNITION_MODEL2;

            //CheckFaceOrAdd(client, IMAGE_BASE_URL, RECOGNITION_MODEL1, "1.jpg").Wait();

        }

        

        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public static async Task DetectFaceExtract(IFaceClient client, string url, string recognitionModel)
        {
            Console.WriteLine("========DETECT FACES========");
            Console.WriteLine();


            List<string> imageFileNames = new List<string>
            {
                                "1.jpg",
                                "2.jpg"
            };

            foreach (var imageFileName in imageFileNames)
            {
                IList<DetectedFace> detectedFaces;

                var file = System.IO.File.Open(url + imageFileName, System.IO.FileMode.Open);
                detectedFaces = await client.Face.DetectWithStreamAsync(file,
                        returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                        FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                        FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                        recognitionModel: recognitionModel);

                Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{imageFileName}`.");

                foreach (var face in detectedFaces)
                {
                    Console.WriteLine($"Face attributes for {imageFileName}:");


                    Console.WriteLine($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");


                    List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                    int count = face.FaceAttributes.Accessories.Count;
                    string accessory; string[] accessoryArray = new string[count];
                    if (count == 0) { accessory = "NoAccessories"; }
                    else
                    {
                        for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                        accessory = string.Join(",", accessoryArray);
                    }
                    Console.WriteLine($"Accessories : {accessory}");


                    Console.WriteLine($"Age : {face.FaceAttributes.Age}");
                    Console.WriteLine($"Blur : {face.FaceAttributes.Blur.BlurLevel}");


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
                    Console.WriteLine($"Emotion : {emotionType}");


                    Console.WriteLine($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
                    Console.WriteLine($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                    Console.WriteLine($"Gender : {face.FaceAttributes.Gender}");
                    Console.WriteLine($"Glasses : {face.FaceAttributes.Glasses}");


                    Hair hair = face.FaceAttributes.Hair;
                    string color = null;
                    if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                    HairColorType returnColor = HairColorType.Unknown;
                    double maxConfidence = 0.0f;
                    foreach (HairColor hairColor in hair.HairColor)
                    {
                        if (hairColor.Confidence <= maxConfidence) { continue; }
                        maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                    }
                    Console.WriteLine($"Hair : {color}");


                    Console.WriteLine($"HeadPose : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}");
                    Console.WriteLine($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                    Console.WriteLine($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                    Console.WriteLine($"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
                        $" {string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
                    Console.WriteLine($"Smile : {face.FaceAttributes.Smile}");
                    Console.WriteLine();
                }
            }
        }

        private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, Stream url, string RECOGNITION_MODEL1)
        {
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithStreamAsync(url, recognitionModel: RECOGNITION_MODEL1);
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{(url as FileStream).Name.Split('\\')[5]}`");
            return detectedFaces.ToList();
        }

        static string personGroupId;
        static string personGroupId1;
        static List<Person> AllGood = new List<Person>();
        static List<Person> AllBad = new List<Person>();
        public  async Task IdentifyInPersonGroup(IFaceClient client, string url, string rECOGNITION_MODEL1)
        {
            Console.WriteLine("========IDENTIFY FACES========");
            Console.WriteLine();


            Dictionary<string, string[]> personDictionary =
                new Dictionary<string, string[]>
                    { { "Good", new[] { "1.jpg" } },
                      { "Bad", new[] { "2.jpg"} }
                    };

            string sourceImageFileName = "1.jpg";

            personGroupId = Guid.NewGuid().ToString();
            sourcePersonGroup = personGroupId; // This is solely for the snapshot operations example
            Console.WriteLine($"Create a person group ({personGroupId}).");
            await client.PersonGroup.CreateAsync(personGroupId, "Good Person", recognitionModel: "recognition_02");
            personGroupId1 = Guid.NewGuid().ToString();
            await client.PersonGroup.CreateAsync(personGroupId1, "BadPerson", recognitionModel: "recognition_02");
            string personGroupIdCurrent = personGroupId;
            List<Person> uu = AllGood;
            foreach (var groupedFace in personDictionary.Keys)
            {
                await Task.Delay(250);

                Console.WriteLine($"Create a person group person '{groupedFace}'.");

                foreach (var similarImage in personDictionary[groupedFace])
                {

                    Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupIdCurrent, name: similarImage);
                    Console.WriteLine($"Add face to the person group person({groupedFace}) from image `{similarImage}`");
                    var file = File.Open(url + similarImage, FileMode.Open);
                    PersistedFace face = await client.PersonGroupPerson.AddFaceFromStreamAsync(personGroupIdCurrent, person.PersonId,
                        file, similarImage);
                    uu.Add(person);
                }
                personGroupIdCurrent = personGroupId1;
                uu = AllBad;
            }

            Console.WriteLine();
            Console.WriteLine($"Train person group {personGroupId}.");
            await client.PersonGroup.TrainAsync(personGroupId);

            Console.WriteLine();
            Console.WriteLine($"Train person group {personGroupId1}.");
            await client.PersonGroup.TrainAsync(personGroupId1);

            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
                Console.WriteLine($"Training status: {trainingStatus.Status}.");
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId1);
                Console.WriteLine($"Training status: {trainingStatus.Status}.");
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
            Console.WriteLine();
            List<Guid> sourceFaceIds = new List<Guid>();
            var file1 = File.Open(url + sourceImageFileName, FileMode.Open);
            List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, file1, recognitionModel);

            foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

            var identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);
            var identifyResults1 = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId1);

            foreach (var identifyResult in identifyResults)
            {
                Person person;
                try
                {

                    person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);

                    Console.WriteLine($"Person '{person.Name}' is identified for face in: {sourceImageFileName} - {identifyResult.FaceId}, is 'Good Person'," +
                        $" confidence: {identifyResult.Candidates[0].Confidence}.");

                }
                catch
                {
                }

            }
            foreach (var identifyResult in identifyResults1)
            {
                Person person;
                try
                {
                    person = await client.PersonGroupPerson.GetAsync(personGroupId1, identifyResult.Candidates[0].PersonId);
                    Console.WriteLine($"Person '{person.Name}' is identified for face in: {sourceImageFileName} - {identifyResult.FaceId}, is 'Bad Person'," +
                        $" confidence: {identifyResult.Candidates[0].Confidence}.");
                }
                catch
                {
                }

            }

            Console.WriteLine();
        }
        public struct MyStruct
        {
            public List<Guid> sourceFaceIds;
            public IFaceClient client;
            public string url;
            public string recognitionModel;
            public string sourceImageFileName;
        }
        public static Nullable<MyStruct> MyStructee { get; set; }
        public static string NameNewPerson { get; set; }
        public  async Task CheckFaceOrAdd(string url, string sourceImageFileName)
        {

            List<Guid> sourceFaceIds = new List<Guid>();
            List<Guid> sourceFaceIds2 = new List<Guid>();
            var file1 = File.Open(url + sourceImageFileName, FileMode.Open);
            List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, file1, recognitionModel);

            foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

            bool G = false;
            bool B = false;
            var identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);
            var identifyResults1 = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId1);

            foreach (var identifyResult in identifyResults)
            {
                Person person;
                try
                {

                    person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);
                    NameNewPerson = person.Name;

                }
                catch
                {
                    G = true;
                }

            }
            foreach (var identifyResult in identifyResults1)
            {
                Person person;
                try
                {
                    person = await client.PersonGroupPerson.GetAsync(personGroupId1, identifyResult.Candidates[0].PersonId);
                    NameNewPerson = person.Name;
                }
                catch
                {
                    B = true;
                }

            }
            if (G && B)
            {
                MyStructee = new MyStruct { client = client, recognitionModel = recognitionModel, sourceFaceIds = sourceFaceIds, sourceImageFileName = sourceImageFileName, url = url };
                NameNewPerson = "";
            }
            else
            {
                MyStructee = null;

            }
        }
        public  async void AddPerson(MyStruct myStruct, string PersonName)
        {

            double t;
            double maxG = 0;
            double maxB = 0;
            foreach (var el in AllGood)
            {
                t = myStruct.client.Face.VerifyFaceToPersonAsync(myStruct.sourceFaceIds[0], el.PersonId, personGroupId).Result.Confidence;
                if (t > maxG) maxG = t;
            }
            foreach (var el in AllBad)
            {
                t = myStruct.client.Face.VerifyFaceToPersonAsync(myStruct.sourceFaceIds[0], el.PersonId, personGroupId1).Result.Confidence;
                if (t > maxB) maxB = t;
            }
            string personGroupIdCurrent = maxG > maxB ? personGroupId : personGroupId1;

            string groupedFace;
            if (personGroupIdCurrent == personGroupId) groupedFace = "Good"; else groupedFace = "Bad";
            Person person = await myStruct.client.PersonGroupPerson.CreateAsync(personGroupId: personGroupIdCurrent, name: PersonName);
            var file = File.Open(myStruct.url + myStruct.sourceImageFileName, FileMode.Open);
            PersistedFace face = await myStruct.client.PersonGroupPerson.AddFaceFromStreamAsync(personGroupIdCurrent, person.PersonId,
                file, myStruct.sourceImageFileName);
            await myStruct.client.PersonGroup.TrainAsync(personGroupIdCurrent);

        }

    }
}
