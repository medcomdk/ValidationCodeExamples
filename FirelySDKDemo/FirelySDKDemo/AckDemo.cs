using System.Text;
using System.Text.Json;
using Firely.Fhir.Packages;
using Firely.Fhir.Validation;
using FirelySDKDemo.Utilities;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Task = System.Threading.Tasks.Task;

namespace FirelySDKDemo;

public class AckDemo
{
    public static async Task Run()
    {
        var packageServer = "https://packages.simplifier.net";
        var packageResolver = new FhirPackageSource(ModelInfo.ModelInspector, packageServer,
            ["medcom.fhir.dk.acknowledgement@2.0.3"]);

        var resourceResolver = await CachedResolveUtilities.GetResourceResolverWithFixedFhirPaths(packageResolver);
        
        var localTerminologyService = new LocalTerminologyService(resourceResolver);
        var mimeTypeTerminologyService = new MimeTypeTerminologyService();
        var externalTerminologyService = new ExternalTerminologyService(new FhirClient("https://tx.hl7europe.eu/r4"));
        var terminologyService = new MultiTerminologyService(localTerminologyService, mimeTypeTerminologyService,
            externalTerminologyService);

        var validator = new Validator(resourceResolver, terminologyService);

        var msg =
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", "Acknowledgement",
                "Acknowledgement-positive.json"));
        var messageBundle = FhirJsonDeserializer.DEFAULT.Deserialize<Bundle>(msg);

        Console.WriteLine("Deserialized Message. Starting validation");
        var validationOutCome = validator.Validate(messageBundle);

        Console.WriteLine($"Amount of Errors: {validationOutCome.Errors}");
        Console.WriteLine($"Amount of Warnings: {validationOutCome.Warnings}");

        foreach (var issue in validationOutCome.Issue)
        {
            if (issue.Expression?.Any() == true)
                Console.WriteLine("  expr: " + string.Join(" | ", issue.Expression));
            if (issue.Location?.Any() == true)
                Console.WriteLine("  loc:  " + string.Join(" | ", issue.Location));
        }
    }
}