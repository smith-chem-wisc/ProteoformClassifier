# ProteoformClassifier
Classifies proteoform identifications and validates that proteoform results are transparent about ambiguity.
Check out the [wiki page](https://github.com/smith-chem-wisc/ProteoformClassifier/wiki) for software details!


## Quick start guide

1. Download the most recent version of ProteoformClassifier [here](https://github.com/smith-chem-wisc/ProteoformClassifier/releases).
"GUI.zip" contains a helpful user-interface. If you're too cool for user-interfaces, you can instead download "CMD.zip" to run ProteoformClassifier from the command line.
This quick start guide will focus on GUI.zip.

2. Unzip "GUI.zip" and run "GUI.exe".
<img src ="https://user-images.githubusercontent.com/16883585/118153916-6bf3fe80-b3e4-11eb-88e5-ff388cc1aee4.png">

3. On the left side of the screen, click on "Validate Input".
<img src ="https://user-images.githubusercontent.com/16883585/118154223-ca20e180-b3e4-11eb-8b01-715534420b3d.png">

4. This page allows you to validate if your proteoform identification software reports ambiguous proteoform identifications. To validate your software, analyze the test file  [Validation.mzML](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Validation.mzML) with your identification software using the protein database [Validation.fasta](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Validation.fasta) and the parameters described in "[README_Parameters.txt](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/README_Parameters.txt)". These files can be downloaded by right-clicking on them and selecting "Save as...". Once you've analyzed these data with your identification software, format your output into a .tsv file containing the scan number, proteoform sequence(s), and gene(s) of origin. An example .tsv produced from [MetaMorpheus](https://github.com/smith-chem-wisc/MetaMorpheus) output is available as "[Results.tsv](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Results.tsv)".

5. Drag and drop "Results.tsv" onto the "Validate Input" tab. Press "Validate Results". If the output is transparent about ambiguity, then the Output Terminal at the bottom of the page will say "Success!". If not, it will provide error messages to assist you in determining which PrSMs and ambiguities are missing. 
<img src ="https://user-images.githubusercontent.com/16883585/118158024-44ebfb80-b3e9-11eb-9878-9755828f4533.png">

6. After validating that your software is transparent about reporting ambiguity, you can classify output using the "Classify PrSMs" tab on the left side of the window. This module should appear similar to the "Validate Input" tab, but it accepts multiple input files and writes classified proteoform result files.
<img src ="https://user-images.githubusercontent.com/16883585/118158419-be83e980-b3e9-11eb-9d35-419fbf410a48.png">

7. The "Classify PrSMs" workflow produces two different output files: "ClassifiedResults" and "ClassifiedSummary".

8. "ClassifiedResults" contains all of the original input data and adds an additional fourth column containing the classification level.
<img src ="https://user-images.githubusercontent.com/16883585/118159005-816c2700-b3ea-11eb-90a2-dbe30c2a0528.png">

9.  "ClassifiedSummary" contains a brief synopsis of how many PrSMs were identified at each level.
<img src ="https://user-images.githubusercontent.com/16883585/118159097-9c3e9b80-b3ea-11eb-8a1f-8a0eaab6956a.png">

Thanks for trying out ProteoformClassifier! If you have any questions, please check out our [wiki](https://github.com/smith-chem-wisc/ProteoformClassifier/wiki) or open an [issue](https://github.com/smith-chem-wisc/ProteoformClassifier/issues) and we'll get back to you ASAP.
