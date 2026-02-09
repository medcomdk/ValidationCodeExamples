using Firely.Fhir.Packages;
using Firely.Fhir.Validation;
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
        var fhirRelease = FhirRelease.R4;
        var packageResolver = new FhirPackageSource(ModelInfo.ModelInspector, packageServer,
            ["medcom.fhir.dk.acknowledgement@2.0.3"]);

        var resourceResolver = await GetResourceResolverWithFixedGlnInvariants(packageResolver);
        
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
    
    static async Task<CachedResolver> GetResourceResolverWithFixedGlnInvariants(FhirPackageSource packageResolver)
    {
        var canonical = "http://hl7.dk/fhir/core/StructureDefinition/dk-core-gln-identifier";
        var original = await packageResolver.ResolveByCanonicalUriAsync(canonical) as StructureDefinition
                       ?? throw new InvalidOperationException($"Cannot resolve {canonical}");

        var patched = original.DeepCopy();
        PatchElements(patched.Snapshot?.Element);
        PatchElements(patched.Differential?.Element);

        var overlay = new InMemoryResourceResolver(patched);

        var combined = new MultiResolver(overlay, packageResolver);

        var resourceResolver = new CachedResolver(combined);
        
        return resourceResolver;
        
        void PatchElements(IEnumerable<ElementDefinition>? elements)
        {
            foreach (var ed in elements ?? Enumerable.Empty<ElementDefinition>())
            {
                foreach (var c in ed.Constraint ?? Enumerable.Empty<ElementDefinition.ConstraintComponent>())
                {
                    if (c.Key is "gln-length" or "gln-modulus-10" && !string.IsNullOrEmpty(c.Expression))
                    {
                        c.Expression = c.Expression
                            .Replace("value.matches(", "matches(")
                            .Replace("value.substring(", "substring(");
                    }
                }
            }
        }
    }
}