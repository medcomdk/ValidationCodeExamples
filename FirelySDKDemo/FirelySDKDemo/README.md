As can be seen in the .csproj file this is a net10.0 project

Example of output:

```
It took 0,1996171 seconds to create FHIR Package Source
It took 0,0013422 seconds to create Local Terminology Service
It took 0,0073779 seconds to create Validator
It took 0,1489004 seconds to deserialize message
It took 47,2213384 seconds to validate the first time
BONUS - It took 0,0170573 seconds to validate a second time
Amount of Errors: 0
Amount of Warnings: 3
IssueType: BusinessRule - Issue details Evaluation of FhirPath for constraint 'medcom-messaging-1' failed: Invocation of function 'conformsTo' failed: The conformsTo() function is not supported in the .NET FhirPath engine.
IssueType: BusinessRule - Issue details Evaluation of FhirPath for constraint 'medcom-careCommunication-1' failed: Invocation of function 'conformsTo' failed: The conformsTo() function is not supported in the .NET FhirPath engine.
IssueType: BusinessRule - Issue details Evaluation of FhirPath for constraint 'medcom-careCommunication-3' failed: Invocation of function 'all' failed: Invocation of function 'conformsTo' failed: The conformsTo() function is not supported in the .NET FhirPath engine.
```
We are currently developing a new patch of our IGs to resolve the BusinessRule issue types of conformsTo

