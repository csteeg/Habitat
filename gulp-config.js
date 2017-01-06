module.exports = function () {
  var instanceRoot = ".";
  var config = {
    websiteRoot: instanceRoot + "\\build\\Website",
    sitecoreLibraries: instanceRoot + "\\build\\Website\\bin",
    licensePath: instanceRoot + "\\Data\\license.xml",
    solutionName: "Habitat",
    buildConfiguration: "Debug",
    runCleanBuilds: false
  };
  return config;
}