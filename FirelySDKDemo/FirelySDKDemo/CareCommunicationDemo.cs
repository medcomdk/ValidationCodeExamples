using System.Text.Json;
using FirelySDKDemo.Utilities;

namespace FirelySDKDemo;

using System.Diagnostics;
using Firely.Fhir.Packages;
using Firely.Fhir.Validation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Terminology;

public class CareCommunicationDemo
{
    public static async System.Threading.Tasks.Task Run()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Step one, create your FHIR Package source:
        // Here we use NPM, there are other ways to do it as well https://docs.fire.ly/projects/Firely-NET-SDK/en/latest/resource-resolving/fhirpackagesource.html#the-fhir-package-source
        // Keep in mind that if you want local copies of the packages, you need to not only download the .tgz package of the standard you are working with, but also all of it's dependencies
        // This is because when you use a Package server it utilizes the npm dependency management system, and automatically downloads all the dependencies an IG has. This you will have to do by hand.
        // You can find all the required dependencies on the IG "Dependencies" tree page, or in the .tgz package.json file. i.e. for carecommunication 5.0.1: 
        /*
            "dependencies" : {
            "hl7.fhir.r4.core" : "4.0.1",
            "hl7.terminology.r4" : "6.5.0",
            "hl7.fhir.uv.extensions.r4" : "5.2.0",
            "hl7.fhir.dk.core" : "3.4.0",
            "medcom.fhir.dk.core" : "3.0.1",
            "medcom.fhir.dk.messaging" : "4.0.1",
            "medcom.fhir.dk.terminology" : "2.0.1",
            "medcom.fhir.dk.messagedefinitions" : "1.0.0"
          },
         */
        // ALSO: This requires you to download the Firely.Fhir.Packages nuget package
        var packageServer = "https://packages.simplifier.net";
        var packageResolver = new FhirPackageSource(ModelInfo.ModelInspector, packageServer,
            ["medcom.fhir.dk.carecommunication@5.0.1"]);
        Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds to create FHIR Package Source");
        stopwatch.Restart();

        // Step two, create your cache resolver, enabling you to cache the IG profiles, valuesets etc.
        var resourceResolver = await CachedResolverUtilities.GetResourceResolverWithFixedFhirPaths(packageResolver);
        // Step three, create your terminology service. Perhaps for your setup you only need the LocalTerminologyService, but it's always good to have a backup, as some of MedCom's
        // IG's requires an external terminology service. If your environment doesn't allow an external terminology service, please contact MedCom for discussion.
        // See more info here https://docs.fire.ly/projects/Firely-NET-SDK/en/latest/validation/terminology-service.html#localterminologyservice
        var localTerminologyService = new LocalTerminologyService(resourceResolver);
        // The MimeType terminology service is quite frequently used, but is external, so including it here avoids having to go through the external terminology service
        var mimeTypeTerminologyService = new MimeTypeTerminologyService();
        var externalTerminologyService = new ExternalTerminologyService(new FhirClient("https://tx.hl7europe.eu/r4"));
        var terminologyService = new MultiTerminologyService(localTerminologyService, mimeTypeTerminologyService,
            externalTerminologyService);
        Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds to create Local Terminology Service");
        stopwatch.Restart();

        // Step four, create your actual validator, the one that does all the hard work. Please note that this validator should
        // be reused across your validations, as it will utilize the cache and have a much greater speed than if you were to recreate it everytime
        // ALSO: this requires you to download the Firely.Fhir.Validation.R4 nuget package
        var validator = new Validator(resourceResolver, terminologyService);
        Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds to create Validator");
        stopwatch.Restart();

        // Step five, deserialize your message. In this case we just read some carecommunication message from the local filesystem
        var someCareCommunicationMsg =
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources","CareCommunication", "1SomeCareCommunicationMsg.xml"));
        // and then deserialize it to a Bundle type
        var messageBundle = FhirXmlDeserializer.DEFAULT.Deserialize<Bundle>(someCareCommunicationMsg);
        Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds to deserialize message");

        // Step six, validate your message
        var validationOutCome = validator.Validate(messageBundle);
        Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds to validate the first time");
        
        // print out the errors and warnings
        Console.WriteLine($"Amount of Errors: {validationOutCome.Errors}");
        Console.WriteLine($"Amount of Warnings: {validationOutCome.Warnings}");

        foreach (var issue in validationOutCome.Issue)
        {
            Console.WriteLine(JsonSerializer.Serialize(issue));
            var coding = issue.Details?.Coding?.FirstOrDefault();
            Console.WriteLine($"IssueCode: {issue} - Issue details {issue.Details?.Text} - Error Code : {coding?.CodeElement?.Value}");
        }

        // Bonus step, see cache in action, deserialize another message with the same validator.
        var anotherCareCommunicationMsg =
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources","CareCommunication", "2AnotherCareCommunicationMsg.xml"));
        var anotherMessageBundle = FhirXmlDeserializer.DEFAULT.Deserialize<Bundle>(anotherCareCommunicationMsg);

        stopwatch.Restart();
        var anotherValidationOutCome = validator.Validate(anotherMessageBundle);
        Console.WriteLine($"BONUS - It took {stopwatch.Elapsed.TotalSeconds} seconds to validate a second time");

        // Bonus step, print out the errors and warnings
        Console.WriteLine($"Amount of Errors: {anotherValidationOutCome.Errors}");
        Console.WriteLine($"Amount of Warnings: {anotherValidationOutCome.Warnings}");

        foreach (var issue in anotherValidationOutCome.Issue)
            var coding = issue.Details?.Coding?.FirstOrDefault();
            Console.WriteLine($"IssueCode: {issue} - Issue details {issue.Details?.Text} - Error Code : {coding?.CodeElement?.Value}");


        // NOTE: Please take care to read the documentation. For example the IExternalReferenceResolver might be very important
        // for your specific use case. As of writing this (6. february 2026) MedCom does not have any IG's that require it

        // Bonus step, json and terminology service. The 3AthirdCareCommunicationMsg.json file needs a different parser and also the mimetype terminology service
        Console.WriteLine(
            "\nA third JSON carecommunication message with a required terminology service to expand the image/png code");
        var aThirdCareCommunicationMsg =
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources","CareCommunication", "3AThirdCareCommunicationMsg.json"));
        var aThirdMessageBundle = FhirJsonDeserializer.DEFAULT.Deserialize<Bundle>(aThirdCareCommunicationMsg);
        var aThirdValidationOutCome = validator.Validate(aThirdMessageBundle);

        Console.WriteLine($"Amount of Errors: {aThirdValidationOutCome.Errors}");
        Console.WriteLine($"Amount of Warnings: {aThirdValidationOutCome.Warnings}");

        foreach (var issue in aThirdValidationOutCome.Issue)
            var coding = issue.Details?.Coding?.FirstOrDefault();
            Console.WriteLine($"IssueCode: {issue} - Issue details {issue.Details?.Text} - Error Code : {coding?.CodeElement?.Value}");
    }
}