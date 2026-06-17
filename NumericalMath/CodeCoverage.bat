@echo off

set finalDirectory="TestResults"

dotnet test --collect "Code Coverage" --results-directory "%finalDirectory%"

dotnet-coverage merge "%finalDirectory%/**/*.coverage" --output "%finalDirectory%/coverage.xml" --output-format xml

reportgenerator -reports:"%finalDirectory%/coverage.xml" -targetdir:"%finalDirectory%/coverage" -reporttypes:Html 

start "" "%finalDirectory%/coverage/index.html"