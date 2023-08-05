# Transmorph Patching System
Pointer-driven expansion patching system

Tesseract Patching System is an universal patching system for Windows X64.

While this program is clearly inspired by Lunar IPS, it fundamentally functions much much differently.

This program generates a Tesseract (TPS Files) from an Input fine and an Output fule. The resulting object contains 2 tables of pointers, one of which points to Raw data from the input file and the other which also points to Raw data but introduces a differential value.

The resulting Tesseract will most likely be bigger in size than the expected Outpuf file. This is normal expected behavior as the transformation operations are bigger than the Output file itself.

Because of the nature of the data contained inside the tesseract, ABSOLUTELY NO DATA FROM THE ORIGINAL FILE IS CONTAINED IN THE PATCH. THE TRANSFORMATION 100% DEPENDS ON THE INPUT FILE'S INTEGRITY.

