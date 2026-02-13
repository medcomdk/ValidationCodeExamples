using Firely.Fhir.Packages;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;

namespace FirelySDKDemo.Utilities;

public static class CachedResolverUtilities
{
    // NOTE: Can easily be made non async
    public static async Task<CachedResolver> GetResourceResolverWithFixedFhirPaths(FhirPackageSource packageResolver)
    {
        var canonicalsToPatch = new[]
        {
            "http://hl7.dk/fhir/core/StructureDefinition/dk-core-gln-identifier",
            "http://hl7.dk/fhir/core/StructureDefinition/dk-core-x-ecpr-identifier"
        };
        
        var patchedResources = new List<StructureDefinition>();

        foreach (var canonical in canonicalsToPatch)
        {
            var original = await packageResolver.ResolveByCanonicalUriAsync(canonical) as StructureDefinition
                           ?? throw new InvalidOperationException($"Cannot resolve {canonical}");

            var patched = original.DeepCopy();

            PatchElements(patched.Snapshot?.Element);
            PatchElements(patched.Differential?.Element);

            patchedResources.Add(patched);
        }

        var overlay = new InMemoryResourceResolver(patchedResources);
        var combined = new MultiResolver(overlay, packageResolver);

        return new CachedResolver(combined);
        
        void PatchElements(IEnumerable<ElementDefinition>? elements)
        {
            foreach (var ed in elements ?? [])
            foreach (var c in ed.Constraint)
            {
                if (string.IsNullOrEmpty(c.Expression)) continue;

                if (c.Key is "gln-length" or "gln-modulus-10" or "x-ecpr")
                {
                    c.Expression = c.Expression
                        .Replace("value.matches(", "matches(")
                        .Replace("value.substring(", "substring(");
                }
            }
        }
    }
}