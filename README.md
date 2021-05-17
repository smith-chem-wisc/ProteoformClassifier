# ProteoformClassifier
Classifies proteoform identifications and validates that proteoform results are transparent about ambiguity.
Check out the [wiki page](https://github.com/smith-chem-wisc/ProteoformClassifier/wiki) for software details!


## Quick start guide

1. Download the most recent version of ProteoformClassifier [here](https://github.com/smith-chem-wisc/ProteoformClassifier/releases).
"GUI.zip" contains a helpful user-interface. If you're too cool for user-interfaces, you can instead download "CMD.zip" to run ProteoformClassifier from the command line.
This quick start guide will focus on GUI.zip.


2. Unzip "GUI.zip" and run "GUI.exe". You may be asked to install .Net
<img src ="https://user-images.githubusercontent.com/16883585/118153916-6bf3fe80-b3e4-11eb-88e5-ff388cc1aee4.png">


3. On the left side of the new GUI screen, click on "Validate Software". This page allows you to validate if your proteoform identification software reports ambiguous proteoform identifications. 
<img src ="https://user-images.githubusercontent.com/16883585/118176571-943d2680-b3ff-11eb-9ae8-5b48d8aec2ac.png">


4. OPTIONAL: To validate your top-down search software, analyze the test file  [Validation.mzML](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Validation.mzML) with your identification software using the protein database [Validation.fasta](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Validation.fasta) and the parameters described in "[README_Parameters.txt](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/README_Parameters.txt)". These files can be downloaded by right-clicking on them and selecting "Save link as...". 


5. If you analyzed the Validation.mzML data file with your identification software, format your search output into a .tsv file containing the scan number, proteoform sequence(s), and gene(s) of origin. If you skipped step #4, you can download and use the formatted "[Results.tsv](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Results.tsv)" produced from [MetaMorpheus](https://github.com/smith-chem-wisc/MetaMorpheus) output. 


6. Drag and drop the formatted results ("Results.tsv") onto the GUI window. Press "Validate Software". If the output is transparent about ambiguity, then the Output Terminal at the bottom of the page will say "Success!". If not, it will provide error messages to assist you in determining which PrSMs and ambiguities are missing. 
<img src ="https://user-images.githubusercontent.com/16883585/118176658-b5057c00-b3ff-11eb-88c8-3e88af541a39.png">


7. After validating that your software is transparent about reporting ambiguity, you can classify proteoform output from that software using the "Classify PrSMs" tab on the left side of the window. This module should appear similar to the "Validate Software" tab, but it accepts multiple input files and writes classified proteoform result files. NOTE: If you do not switch to the Classify PrSMs tab and attempt to run full dataset results on the Validate Software tab, you will get many useless error messages. The Validate Software tab is only for the [Validation.mzML](https://raw.githubusercontent.com/smith-chem-wisc/ProteoformClassifier/main/Test/ValidationFiles/Validation.mzML) data.
<img src ="https://user-images.githubusercontent.com/16883585/118176863-f138dc80-b3ff-11eb-975b-c25d0febe3b2.png">


8. The "Classify PrSMs" workflow produces two different output files: "ClassifiedResults" and "ClassifiedSummary".


9. "ClassifiedResults" contains all of the original input data and adds an additional fourth column containing the classification level for each PrSM.
<img src ="https://user-images.githubusercontent.com/16883585/118159005-816c2700-b3ea-11eb-90a2-dbe30c2a0528.png">


10.  "ClassifiedSummary" contains a brief synopsis of how many PrSMs were identified at each level.
<img src ="https://user-images.githubusercontent.com/16883585/118159097-9c3e9b80-b3ea-11eb-8a1f-8a0eaab6956a.png">


Thanks for trying out ProteoformClassifier! If you have any questions, please check out our [wiki](https://github.com/smith-chem-wisc/ProteoformClassifier/wiki) or open an [issue](https://github.com/smith-chem-wisc/ProteoformClassifier/issues) and we'll get back to you ASAP.
