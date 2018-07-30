USE [Anlab]
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (200, N'Soils', N'# Saturated Paste Extract: SP
## Summary
This method involves saturating the soil with water and subsequent extraction under partial vacuum of the liquid phase for the determination of dissolved salts. Soil moisture at the point of complete saturation is the maximum amount of water held when all the soil pore space is occupied by water and when no free water has collected on the surface of the paste. Over a wide soil textural range, the saturation percentage (SP) is approximately twice the Field Capacity (FC) or -33kPa soil water potential and is four times the Permanent Wilting Point (PWP) or -1500 kPa soil water potential for soils of loam, to clay loam texture. The soil pH may be determined directly on the paste. From the saturated paste extract, estimates of ECe, and solution concentrations of Ca^2+^, Mg^2+^, K ^+^,  Na ^+^, Cl ^-^, B, HCO~3~^-^, CO~3~^2-^, SO~4~ estimate (actual measurement is total S in extracts), SAR and ESP can be made. The method is generally reproducible within 8%.

Rhoades, J. D. 1982. Soluble salts. p. 167-179. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2: Chemical and microbiological properties. Monograph Number 9 (Second Edition). ASA, Madison, WI.', N'Saturated Paste And Saturation Percentage')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (205, N'Soils', N'# Saturated Paste Extract: pH
## Summary
This method determines the pH of soil, using a saturated paste prepared from the soil and a pH meter. It is most applicable to soils with a pH ranging from 4.0 to 9.0. It is not possible to determine the total acidity or alkalinity of the soil from pH because of the nature of the colloidal system and junction potential. This method does however provide information on the disassociated H-ions affecting the sensing electrode. The method is generally reproducible within 0.2 pH units.

U.S. Salinity Laboratory Staff. 1954. pH reading of saturated soil paste. p. 102. In: L. A. Richards (ed.) Diagnosis and improvement of saline and alkali soils. USDA Agricultural Handbook 60. U.S. Government Printing Office, Washington, D.C.', N'Soil pH')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (210, N'Soils', N'# By Special Request
## Summary
This method determines the pH of soil, using an extract of a 1 to 10 dilution of soil with water and a pH meter. The pH of a soil sample increases with the degree of saturation (dilution effect). This rise in pH from pHs to pH 1+10 is usually 0.2 to 0.5 pH units but may be one or more units in certain alkaline soils where there is an increase in dissociation due to dilution of soluble salts.

Rible, J. M., and Quick, J. 1960. Method S-3.1. In: Water soil plant tissue tentative methods of analysis for diagnostic purposes. Davis, University of California Agricultural Experiment Service. Mimeographed Report.', N'pH of a 1 + 10 Soil Suspension')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (215, N'Soils', N'# Saturated Paste Extract : EC
## Summary
This method semi-quantifies the amounts of soluble salts in the liquid phase extracted from the saturated paste of soils and is based on the measurement of the electrical conductivity (ECe) of a saturated paste extract. The higher the concentration of salt in a solution, the higher will be the electrical conductance (the reciprocal of resistance). Electrical conductivity is a function of quantity and specific types of cations and anions in the extract. Plant tolerance can be related to the ECe value of the saturated paste. The method has a detection limit of approximately 0.01 dS m-1.

Rhoades, J. D. 1982. Soluble salts. p. 167-179. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2: Chemical and microbiological properties. Monograph Number 9 (Second Edition). ASA, Madison, WI.', N'Estimated Soluble Salts (ECe)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (220, N'Soils', N'# Saturated Paste Extract: HCO~3~, CO~3~
## Summary
This method quantifies bicarbonate (HCO~3~-) and carbonate (CO~3~^2-^) levels in a soil water extract, such as from saturated paste extract. Quantitation is by titration with 0.025 N H~2~SO~4~. The measurement should be made immediately due to the potential of the extract being super- saturated relative to calcium carbonate (CaCO~3~). The method has a detection limit of approximately 0.1 meq/L.

Sample amount requested:   300 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

U.S. Salinity Laboratory Staff. 1954. Carbonate and bicarbonate by titration with acid. p. 98. In: L. A. Richards (ed.) Diagnosis and improvement of saline and alkali soils. USDA Agricultural Handbook 60. U.S. Government Printing Office, Washington, D.C.', N'Bicarbonate And Carbonate In Saturated Paste Extract')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (227, N'Soils', N'# Saturated Paste Extract: Cl
## Summary
This method quantifies the amount of Cl^-^ in a soil water extract, such as from the saturated paste extract. Thiocyanate ion is liberated from mercuric thiocyanate by the formation of soluble mercuric chloride. In the presence of ferric ion, free thiocyanate ion forms the highly colored ferric thiocyanate, of which the absorbance is proportional to the chloride concentration. The absorbance of the ferric thiocyanate is read at 480 nm. Plant tolerance to chloride can be related to the concentration of chloride in the saturated paste extract. The method has a detection limit of 0.1 meq/L of Cl^-^ and is generally reproducible within 5%.

Diamond, D. 2001. Determination of Chloride by Flow Injection Analysis Colorimetry. QuikChem Method 10-117-07-1-H. Lachat Instruments, Milwaukee, WI.

Rhoades, J. D. 1982. Soluble salts. p. 167-179. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2: Chemical and microbiological properties. Monograph Number 9 (Second Edition). ASA, Madison, WI.', N'Chloride In Saturated Paste Extract - Flow Injection Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (235, N'Soils', N'# Saturated Paste Extract: K, Na, Ca, Mg, B and SO~4~-S
## Summary
This method quantitatively determines the concentration of K, Na, B, S, Ca, and Mg in the saturated paste extract using Inductively Coupled Plasma Emission Spectrometry (ICP-AES) for Ca, B, S and Mg and flame Atomic Emission Spectrometry (AES) for K and Na. Sulfur results are assuming that all sulfur present is in the sulfate from. Sulfate uptake by plants can be related to the sulfate concentration in the saturated paste extract. Plant tolerance to soil boron can be related to the boron concentration in the saturated paste extract. K, Na, Ca and Mg are generally the dominant cations in the saturated paste extract of soils. Concentration of soluble Na, Ca, and Mg is used to determine the sodium adsorption ratio (SAR) of soils. Extract solutions containing greater than 10,000 mg/L (1.0% w/v, estimated from ECe) will require dilution since solutions of this salt concentration may impair instrument operation.

|Element | MDL |
|------------|----------|
|B| 0.05 mg/L|
|Ca|0.10 meq/L|
|K| 0.1 mg/L|
|Mg|0.10 meq/L|
|Na| 0.10 meq/L|
|S|0.1 mg/L|

<br />
Sample amount requested: 200 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Knudsen, D., Peterson, G. A. and Pratt, P. F. 1982. Lithium, sodium and potassium. pp. 225-246. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2. Chemical and microbiological properties. ASA Monograph Number 9.

Soltanpour, P. N. Benton Jones, Jr., J. and S. M. Workman. 1982. Optical emission spectrometry. p. 29-65. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2. Chemical and microbiological properties. Monograph Number 9 (Second Edition). ASA, Madison, WI.', N'Potassium, Sodium, Calcium, Magnesium, Boron and sulfate-Sulfur In Saturated Paste Extract')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (240, N'Soils', N'# Saturated Paste Extract: SAR, ESP
## Summary
The exchangeable sodium percentage (ESP) and the sodium adsorption ratio (SAR) are calculated after determining Ca, Mg and Na concentrations in a saturation extract (SOP# 235).

U.S. Salinity Laboratory Staff. 1954. Choice of determinations and interpretation of data. p. 26. In: L. A. Richards (ed.) Diagnosis and improvement of saline and alkali soils. USDA Agric. Handb. 60. U.S. Government Printing Office, Washington, D.C.', N'SAR (Sodium Adsorption Ratio) and ESP (Exchangeable Sodium Percentage)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (312, N'Soils', N'# Fertility: NO~3~-N, NH~4~-N
## Summary
This method involves the extraction of nitrate (NO~3~-N) and ammonium (NH~4~-N) from soils using an equilibrium extraction with 2.0 N KCl solution.  Nitrate is determined by reduction to nitrite via a copperized cadmium column.  The nitrite is then determined by diazotizing with sulfanilamide followed by coupling with N-(1-naphthyl) ethylenediamine dihydrochloride.  The absorbance of the product is measured at 520 nm.  Ammonia is determined by heating with salicylate and hypochlorite in an alkaline phosphate buffer.  The presence of EDTA prevents precipitation of calcium and magnesium. Sodium nitroprusside is added to enhance sensitivity.  The absorbance of the reaction product is measured at 660 nm and is directly proportional to the original ammonia concentration.  Extracts can be stored for up to three weeks at low temperature (<4°C) or frozen.  The method has a detection limit of approximately 0.10 ppm (on a soil basis). The results of analysis of un-dried soil are reported on a dry soil basis.  

Sample amount requested:  20 g for nitrate and/or ammonium.  Please contact the Analytical Laboratory with questions concerning limited sample.

Hofer, S. 2003. Determination of Ammonia (Salicylate) in 2M KCl soil extracts by Flow Injection Analysis.  QuikChem Method 12-107-06-2-A. Lachat Instruments, Loveland, CO.

Keeney, D. R. and Nelson, D. W. 1982. Nitrogen-inorganic forms.   pp. 643-698. In: A. L. Page (ed.) Methods of soil analysis: Part 2. Chemical and microbiological properties. ASA Monograph Number 9.

Knepel, K. 2003. Determination of Nitrate in 2M KCl soil extracts by Flow Injection Analysis.QuikChem Method 12-107-04-1-B. Lachat Instruments, Loveland, CO.', N'Soil Nitrate And Extractable Ammonium By Flow Injection Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (315, N'Soils', N'# Fertility: TKN
## Summary
The Total Kjeldahl Nitrogen (TKN) method is based on the wet oxidation of soil organic matter and botanical materials using sulfuric acid and digestion catalyst and conversion of organic nitrogen to the ammonium form. Ammonium is determined using the diffusion-conductivity technique. The procedure does not quantitatively digest nitrogen from heterocyclic compounds (bound in a carbon ring), oxidized forms such as nitrate and nitrite, or ammonium from within mineral lattice structures. The method has a detection limit of approximately 0.001 % N.

Sample amount requested:  3 g

Samples should be powdered and should pass a 1 mm sieve.

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

**Note:** SOP 315 (Soil Method) and SOP 515 (Plant Method) are identical.

Horneck, D. A. and Miller, R. O. 1998. Determination of Total Nitrogen in Plant Tissue pp. 75-83. In: Kalra, Y. P. (ed.) Handbook of Reference Methods for Plant Analysis, CRC Press, New York.

Isaac, R. A. and Johnson, W. C. 1976. Determination of total nitrogen in plant tissue, using a block digestor. J. Assoc. Off. Anal. Chem. 59:1 98-100.', N'Total Kjeldahl Nitrogen (TKN)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (320, N'Soils', N'# Fertility: N, C
## Summary
This analytical method quantitatively determines the total amount of nitrogen and carbon in all forms in soil either using an instrument that utilizes a combustion system with an induction furnace coupled with a thermal conductivity detector (TCD) system and an IR detector system, or, for samples with limited material, an instrument that has a dynamic flash combustion system coupled with a gas chromatographic (GC) separation system and thermal conductivity detection (TCD) system. The analytical method is based on the oxidation of the sample by “flash combustion” which converts all organic and inorganic substances into combustion gases (N~2~, NO~x~, CO~2~, and H~2~O). The method has a detection limit of approximately 0.02% for carbon and nitrogen.

Sample material must be ground to a fineness of < 0.5 mm (60 mesh).  

Sample amount requested:  5 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 972.43, Microchemical Determination of Carbon, Hydrogen, and Nitrogen, Automated Method, in Official Methods of Analysis of AOAC International, 16th Edition (1997), Chapter 12, pp. 5-6, AOAC International, Arlington, VA.', N'Total Nitrogen And Carbon - Combustion Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (322, N'Soils', N'## Summary
This analytical method quantitatively determines the total amount of organic carbon in soil.  The method involves pre-treating the sample with dilute acid to remove carbonate carbon and then analyzing for total carbon using an instrument that utilizes a combustion system with an induction furnace coupled with a thermal conductivity detector (TCD) system and an IR detector system. In cases of very limited sample amount, an alternative method maybe utilized. This method combines acid fumigation with a dynamic flash combustion system that is coupled with a gas chromatographic (GC) separation system and thermal conductivity detection (TCD) system.  Acid fumigation with hydrochloric vapor removes inorganic carbon with no loss of organic carbon. Both analytical methods are based on the oxidation of the sample by “flash combustion” which converts all organic and inorganic substances into combustion gases (N~2~, NO~x~, CO~2~, and H~2~O). The method has a detection limit of approximately 0.02% carbon

Sample material must be ground to a fineness of < 0.5 mm (60 mesh). 

Sample amount requested:  5

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory

Harris, D., Horwáth, W. R.  and Van Kessel, C.  2001.  Acid fumigation of soils to remove carbonates prior to total organic carbon or CARBON-13 isotopic analysis.  Soil Science Society of America Journal 65:1853-1856 (2001.) 

AOAC Official Method 972.43, Microchemical Determination of Carbon, Hydrogen, and Nitrogen, Automated Method, in Official Methods of Analysis of AOAC International, 16th Edition (1997), Chapter 12, pp. 5-6, AOAC International, Arlington, VA.', N'Total Organic Carbon - Combustion Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (330, N'Soils', N'# Fertility: SO~4~-S
## Summary
This method estimates the quantitative concentration of sulfate sulfur (SO~4~-S) in the soil by extraction with monocalcium phosphate. This method for extractable sulfur as SO~4~-S follows the procedure originally outlined by Schulte and Eik (1988) with the following exception: (1) elimination of activated carbon and (2) determination of S by ICP-AES. The ICP-AES determines all sulfur, both organic and inorganic. This method is inappropriate for soil containing greater than 4% organic matter. The method is quantitative only for the time of sampling since sulfur is constantly being mineralized in the soil. The method has a detection limit of approximately 0.5 mg/kg sulfur as sulfate and is generally reproducible within 8%.

Schulte, E. E. and Eik, K. 1988. Recommended sulfate-sulfur test. p. 17-19. In: Recommended chemical soil test procedures for north central region. North Dakota Agric Exp Sta Bull No. 499 (revised).', N'Sulfate - Sulfur')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (335, N'Soils', N'# Fertility: Bray-P
## Summary
This method estimates the relative bioavailability of inorganic ortho-phosphate (PO~4~-P) in soils with acid to neutral pH, using a dilute acid solution of hydrochloric acid containing ammonium fluoride. The orthophosphate ion reacts with ammonium molybdate and antimony potassium tartrate under acidic conditions to form a complex. This complex is reduced with abscorbic acid to form a blue complex which absorbs light at 880 nm. The method is shown to be well correlated to crop response on neutral to acid soils. The absorbance is proportional to the concentration of orthophosphate in the sample. The method has a detection limit of approximately 0.5 mg kg^-1^ (soils basis) and is generally reproducible within 8%.

Diamond, D. 1995. Phosphorus in soil extracts. QuikChem Method 10-115-01-1-A. Lachat Instruments, Milwaukee, WI.

Olsen, S. R. and Sommers, L. E. 1982. Phosphorus. p. 403-430. In: A. L. Page, et al. (eds.) Methods of soil analysis: Part 2. Chemical and microbiological properties. Agron. Mongr. 9. 2nd ed. ASA and SSSA, Madison, WI.', N'Extractable Phosphorus - Bray Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (340, N'Soils', N'# Fertility: Olsen-P
## Summary
This method estimates the relative bioavailability of inorganic ortho-phosphate (PO4-P) in soils with neutral to alkaline pH.  It is not appropriate for soils which are mild to strongly acidic (pH <6.5).  The method is based on the extraction of phosphate from the soil by 0.5 N sodium bicarbonate solution adjusted to pH 8.5.  In the process of extraction, hydroxide and bicarbonate competitively desorb phosphate from soil particles and secondary absorption is minimized because of high pH.  The orthophosphate ion reacts with ammonium molybdate and antimony potassium tartrate under acidic conditions to form a complex.  This complex is reduced with ascorbic acid to form a blue complex which absorbs light at 880 nm.  The absorbance is proportional to the concentration of orthophosphate in the sample. The method has shown to be well correlated to crop response to phosphorus fertilization on neutral to alkaline soils.  The method has a detection limit of 1.0 ppm (soil basis).

Sample amount requested:  5 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Olsen, S. R. and Sommers, L. E. 1982. Phosphorus. pp. 403-430. In: A. L. Page, et al. (eds.) Methods of soil analysis: Part 2. Chemical and microbiological properties. Agron. Mongr. 9. 2nd ed. ASA and SSSA, Madison, WI.

Prokopy, W. R.  1995. Phosphorus in 0.5 M sodium bicarbonate soil extracts. QuikChem Method 12-115-01-1-B.  Lachat Instruments, Milwaukee, WI.', N'Extractable Phosphorus - Olsen Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (360, N'Soils', N'# Fertility: X-K, X-Ca, X-Mg, X-Na, CEC-Estimated
## Summary
This method is semi-quantitative and determines the amount of soil exchangeable K, Ca, Mg, and Na residing on the soil colloid exchange sites by displacement with ammonium acetate solution buffered to pH 7.0.  Cations are quantitated by inductively coupled plasma atomic emission spectrometry (ICP-AES).  Generally, these cations are associated with the exchange capacity of the soil.  The method has detection limits of approximately of 1 ppm or 0.01 meq/100g for each cation.  The estimation of cation exchange capacity is reported as the sum of the results for K, Ca, Mg, and Na. The method does not correct for calcium and magnesium extracted as free carbonates or gypsum.

Sample amount requested:  10 g (for one or up to all four elements)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Thomas, G. W. 1982. Exchangeable cations. pp 159-165. In: A.L. Page et al. (ed.) Methods of soil analysis: Part 2. Chemical and microbiological properties. ASA Monograph Number 9.', N'Exchangeable Potassium, Calcium, Magnesium, Sodium And Estimated Cation Exchange Capacity')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (370, N'Soils', N'# By Special Request
## Summary
As the exchangeable K of soils is removed by cropping or leaching, some K from the mineral reserve becomes exchangeable K. Soils with low exchangeable K content need not be deficient, as the mineral reserve may be able to supply K for plant growth. Extraction of K with H~2~SO~4~ removes an amount of K that seems to be correlated with the removal by extensive cropping. The method has a detection limit of 1 mg kg^-1^ (on a soil basis) and is generally reproducible within 10%.

Brown, A. L., Quick, J. and deBoer, G. J. 1973. Potassium deficiency by soil analysis. p. 13-14. Cal. Agr. June.', N'Potassium -Sulfuric Acid Extraction')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (380, N'Soils', N'# Fertility: Zn, Mn, Cu, Fe
## Summary
The DTPA (diethylenetriaminepentaacetic acid) micronutrient extraction method is a non-equilibrium extraction for estimating the potential soil availability of Zn, Cu, Mn, and Fe. It has been used for cadmium, nickel and lead in soils. The method has shown to be well correlated to crop response to fertilizer for zinc and copper. The amount of micronutrients and trace metals extracted are affected by solution pH, temperature, soil extraction ratio, shaking time, extraction time, and extractant concentration. Extracts are analyzed by ICP-AES or Flame AA. The method has a detection limit of approximately 0.1 mg/kg for Zn, Cu, Mn, and Fe and is generally reproducible within 10% for Cu and Zn and 15% for Fe and Mn. The method is not well characterized for other elements.

Lindsay, W. L. and Norvell, W. A. 1978. Development of a DTPA soil test for zinc, iron, manganese, and copper. Soil Sci. Soc. Amer. J. 42:421-428.', N'Extractable Micronutrients Using Dtpa Extraction - Zinc, Manganese, Copper, And Iron')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (390, N'Soils', N'# Soil Totals: Zn, Mn, Fe, Cu, Cd, Cr, Pb, Ni, P, Mo
## Summary
This method determines the concentration of Cu, Cd, Cr, Fe, Mn, Ni, P, Pb, Zn and additional elements as requested utilizing a nitric acid/hydrogen peroxide closed vessel microwave digestion.  Analysis is by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES).  The approximate method detection limit is 1 mg/kg for all elements except cadmium (0.1 mg/kg) and phosphorus (0.001%). Sample amount requested is 3 g powdered or finely ground sample.

Sah, R. N. and Miller, R. O. 1992. Spontaneous reaction for acid dissolution of biological tissues in closed vessels. Anal. Chem. 64:230-233.', N'Total Elements (Includes Phosphorus, Zinc, Manganese, Iron, Copper, Molybdenum, Cadmium, Chromium, Lead And Nickel)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (392, N'Soils', N'# Fertility: AL-KCL
## Summary
This method is semi-quantitative and determines the amount of 1N KCl extractable aluminum in soil. Aluminum concentration is determined in the extract by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES). The method has a detection limit of approximately 1.0 mg/kg.

Gavlak, R., Horneck, D., Miller, R. O., Kotuby-Amacher, J. 2003. Soil, Plant and Water Reference Methods for the Western Region. p.134-135.', N'KCL Extractable Aluminum')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (394, N'Soils', N'## Summary
This method is quantitative for selenium and is based on the wet oxidation of the sample with nitric, perchloric and sulfuric acids, reduction of selenate to selenite (IV), and determination by vapor generation Inductively-Coupled Plasma Emission Spectrometer (ICP-AES). The method has a detection limit of  0.10 ppm.

Sample should be powdered and should pass a 1 mm sieve.

Sample amount requested:  3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Tracy, M. L. and Moeller, G. 1990. Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis. Part 1: Selenium. J. Assoc. Off. Anal. Chem. 73:404-410.', N'Selenium')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (396, N'Soils', N'## Summary
Arsenic
This method is quantitative for arsenic using digestion with nitric, perchloric and sulfuric acids, reduction of arsenate to arsenite, and determination by vapor generation Inductively-Coupled Plasma Emission Spectrometer (ICP-AES). The method has a detection limit of approximately 0.10 ppm. 

Sample material should be powdered and should pass a 1 mm sieve.

Sample amount requested:  3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Tracy, M. L. and Moeller, G. 1990. Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis. Part 2: Arsenic. J. Assoc. Off. Anal. Chem. 74:516-521.', N'Arsenic')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (410, N'Soils', N'# Alternate Methods: OM (Walkley-Black), Org.C (W-B)
## Summary
This method quantifies the amount of oxidizable organic matter in which OM is oxidized with a known amount of chromate in the presence of sulfuric acid. The remaining chromate is determined spectrophotometrically at 600nm wavelength. The calculation of organic carbon is based on organic matter containing 58% carbon. The method has a detection limit of approximately 0.10% and, on homogeneous sample material, is generally reproducible within 8%.

Samples with concentrations greater than 80% OM are best tested by the Loss-on-Ignition (OM-LOI) method.

Sample amount requested:  10 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Nelson, D. W. and Sommers, L. E. 1982. Total carbon, organic carbon and organic matter. p. 539-579. In: A. L. Page et al. (ed.) Methods of soil analysis: Part 2. Chemical and microbiological properties. ASA Monograph Number 9.', N'Organic Matter - Walkley-Black Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (415, N'Soils', N'# PHYSIO CHEM:  OM (LOI), Org.C (LOI)
## Summary
This method estimates soil organic matter based on gravimetric weight change associated with high temperature oxidation of organic matter.  After initial oven drying at 105ºC, the samples are ignited in a muffle furnace for 2 hours at 360ºC.  The percent weight loss during the ignition step is reported as OM-LOI (% wt. loss) with a method detection limit of 0.05 %.  The calculation of organic carbon is based on the assumption that organic matter contains 58% carbon. 

Note:  To improve the accuracy of estimation of organic matter from loss-on-ignition data, it is recommended that a comparison study of OM by Walkley-Black and LOI be performed to determine the relationship between the two measurements.  The resulting equation can then be used for better estimation of organic matter from LOI values for similar samples.

Sample amount requested:  50 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Nelson, D. W., and Sommers, L. E. 1996. Chapter 34. p 1001-1006. Total Carbon, Organic Carbon, and Organic Matter. In: J. M. Bigham et al. (ed.) Soil Science Society of America and American Society of Agronomy. Methods of Soil Analysis. Part 3. Chemical Methods-SSSA Book Series no. 5. Madison, WI.', N'Organic Matter - Loss-On-Ignition Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (420, N'Soils', N'# By Special Request
## Summary
The procedure is based on the measurement of the loss of crystal water associated with the hydrated form of gypsum. The precise determination of gypsum is difficult because of sources of Ca and SO~4~ other than from gypsum. The method has a detection limit of approximately 0.1meq 100 gm^-1^ and is reproducible within 20%.

Klute, A. 1986. Water retention: laboratory methods. p. 635-662. In: A. Klute (ed.) Methods of soil analysis: Part 1. Physical and mineralogical methods. ASA Monograph Number 9.', N'Gypsum Content')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (430, N'Soils', N'## Summary
The method determines the cation exchange capacity (CEC) of soil.  Barium is used to quantitatively displace soil exchangeable cations.  Four deionized water rinses are used to remove excess barium.  A known quantity of calcium is then exchanged for barium and excess solution calcium is measured.  CEC is determined by the difference in the quantity of the calcium added and the amount found in the resulting solution.  The method has a detection limit of approximately 2.0 meq/100g.

Sample amount requested:  15 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Rible, J. M. and Quick, J. 1960. Method S-19.0. Cation Exchange Capacity.  *In:* Water soil plant tissue.  Tentative methods of analysis for diagnostic purposes. Davis, University of California Agricultural Experiment Service. Mimeographed Report.', N'Cation Exchange Capacity')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (440, N'Soils', N'# Physio Chem: CaCO~3~
## Summary
This method is based on the gravimetric loss of carbonates as carbon dioxide in the presence of excess hydrochloric acid. Major sources of error are: evaporation of water and failure to adequately degas CO~2~ from the sample. Soil carbonates are measured to determine soil buffering capacity with relation to soil fertility, chemical and pedogenic processes.  This procedure is an estimate of free calcium carbonate in the sample. This procedure does not adjust for magnesium, potassium, or sodium carbonates or organic matter which may be present.  The method detection limit is approximately 0.2% CaCO~3~ equivalent (on a dry soil basis).

Sample amount requested:  15 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

U.S. Salinity Laboratory Staff. 1954. Alkaline-earth carbonates by gravimetric loss of carbon dioxide. p. 105. In: L. A. Richards (ed.) Diagnosis and improvement of saline and alkali soils. USDA Agric. Handb. 60. U.S. Government Printing Office, Washington, D.C.', N'Calcium Carbonate Equivalent')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (450, N'Soils', N'# Physio Chem: LiR
## Summary
Buffer pH is the measure of a soil’s active and reserve acidity (i.e., buffer capacity) and is used to estimate lime recommendations.  The method is based on the Shoemaker, McLean and Pratt (SMP) method using the reaction of soil buffered acidity with a chemical buffer resulting in a change in the pH of the buffer.  This method is used for estimating exchange acidity including that associated with exchangeable aluminum.  Standard calibration curves exist for liming based on a SMP value to a desired pH for soil groups.  The lime requirement reported for this test is based on a desired pH of 7.0.  The result is reported in T/A/8in (tons per acre of 100% CaCO~3~ required based on an 8 inch furrow slice weighing 2.4 million pounds).  The table used is from Soil, Plant and Water Reference Methods for the Western Region, 2003.

Sample amount requested:  15 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Shoemaker, H. E., McLean, E. O. and Pratt, P. F. 1961. Buffer methods for determining lime requirement of soils with appreciable amounts of extractable aluminum. Soil Sci. Soc. Am. Proc. 25:274-277.', N'Lime Requirement')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (460, N'Soils', N'# Physio Chem: Moisture Retention
## Summary
The method determines the soil moisture content under constant preset pressure potential (ranging from -10 to -1500 kPa [0.1 atm to 15 atm]). Soil is brought to near saturation and then is allowed to equilibrate under a set atmospheric pressure potential. The method is used to determine the available water capacity of soils and/or moisture release curve. The method detection limit is 0.5%.

Klute, A. 1986. Water retention: laboratory methods. p. 635-662. In: A. Klute (ed.) Methods of soil analysis: Part 1. Physical and mineralogical methods. ASA Monograph Number 9.', N'Moisture Retention')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (470, N'Soils', N'# Particle Size Analysis (Sand/Silt/Clay) 
## Summary
This method quantitatively determines the physical proportions of three sizes of primary soil particles as determined by their settling rates in an aqueous solution using a hydrometer.  The hydrometer method of estimating particle size analysis (sand, silt and clay content) is based on the dispersion of soil aggregates using a sodium hexametaphosphate solution and subsequent measurement based on changes in suspension density.  The method has a detection limit of 1% sand, silt and clay (dry soil basis).

Sample amount requested:  100 g
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Sheldrick, B. H. and Wang, C. 1993. Particle-size Distribution. pp. 499-511. In: Carter, M. R. (ed), Soil Sampling and Methods of Analysis, Canadian Society of Soil Science, Lewis Publishers, Ann Arbor, MI.', N'Particle Size Analysis')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (480, N'Soils', N'# Physio Chem: BD
## Summary
Soil bulk density is the ratio of the mass of dry solids to the bulk volume of the soil. The bulk volume includes the volume of the solids and of the pore space. The determination of bulk density consists of drying (105°C) and weighing a soil sample, the volume of which is known (core method) or must be determined.

Blake, G. R. and Hartge, K. H.  1986. Bulk density. p. 363-375. In: A. Klute et al. (ed.) Methods of soil analysis: Part 1: Physical and Mineralogical Methods. Monograph Number 9 (Second Edition). ASA, Madison, WI.', N'Bulk Density')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (505, N'Plant', N'# Feed: DM
## Summary
This procedure is performed on botanical materials (plant and feed) that have been air- or oven-dried at 55-60ºC and ground, to allow for correction of test results to 100% dry matter.  If the moisture content of the sample as submitted is required, the Partial Dry Matter test should be requested.  The method quantitatively determines the dry matter content based on the gravimetric loss of free water associated with heating to 105ºC for a period of three hours.  The method does not remove molecularly bound water.

Sample amount requested:  1 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

NFTA, Moisture Task Force Report, 2.2.2.5 Laboratory Dry Matter by Oven Drying for 3 hours at 105oC, 2001. pp. 1-3.', N'Dry Matter Determination For Botanical Materials')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (507, N'Plant', N'# Feed:  Partial DM (dried at 55°C)
## Summary
This procedure is performed on botanical materials (plant and feed) and miscellaneous samples such as manure to determine the moisture content of the sample as submitted.

Moisture is evaporated from samples in a forced-air oven at 55-60°C and Partial Dry Matter is determined gravimetrically as the residue remaining after oven drying. Some moisture remains in the sample because drying at this temperature does not remove all water. Drying at higher temperatures (greater than 60°C) causes chemical changes in the sample that affect subsequent testing. The Dry Matter test should also be requested if the total moisture content of the sample following drying at 105°C is required.

Partial drying of certain samples such as manure may not be compatible with subsequent analysis for constituents that might be volatilized or chemically or biologically transformed during drying.

NFTA method 2.2.1.1, Partial Dry Matter Using Forced-air Drying Ovens.', N'Partial Dry Matter')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (510, N'Plant', N'# NO~3~-N, NH~4~-N
## Summary
This method quantitatively extracts soluble nitrate (NO3-N) and ammonium (NH4-N) in botanical materials based on an extraction with a solution of 2% acetic acid.  Ammonium is determined by the diffusion-conductivity method based on the gaseous diffusing of ammonia (NH3) across a gas permeable membrane in the presence of excess base (KOH) and subsequent conductivity detection.  Nitrate is determined by first reducing it to ammonium using a copper-zinc reduction column and subsequent measurement as described above.  Concentrations of these nutrients are used to assess plant overall nitrogen status and are correlated to plant response to nitrogen fertilization.  The method detection limit is 10 ppm (dry basis).

High concentrations of ammonium can interfere with the nitrate quantitation. Volatile amines such as methylamine and ethylamine will interfere.

Sample amount requested:  3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Carlson, R. M., Cabrera, R. I., Paul, J. L., Quick, J. and Evans, R. Y. 1990. Rapid direct determination of ammonium and nitrate in soil and plant tissue extracts. Commun. Soil Sci. Plant Anal. 21:1519-1529.', N'Extractable Nitrate And Ammonium In Botanical Materials - Diffusion Conductivity Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (512, N'Plant', N'# NO~3~-N
## Summary
This method quantitatively measures soluble nitrate (NO3-N) in botanical materials based on an extraction with a solution of 2% acetic acid. Nitrate is determined by Flow Injection Analysis (FIA) using the reduction to nitrite via a copperized cadmium column,   diazotization with sulfanilamide followed by coupling with N-(1-naphthyl)ethlyenediaminie dihydrochloride. The absorbance of the product is measured at 520 nm.

Nitrate concentration is used to assess plant overall nitrogen status and is correlated to plant response to nitrogen fertilization.  The method has a quantitative detection limit of 10 ppm (dry basis).

Sample amount requested:  3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Miller, R. O. 1998.  Extractable Chloride, Nitrate, Orthophosphate, and Sulfate-Sulfur in Plant Tissue: 2% Acetic Acid Extraction, in Handbook of Reference Methods for Plant Analysis, Chapter 15, pp. 115-118.

Sechtig, A.  1992.  Determination of Nitrate/Nitrite by Flow Injection Analysis in 2 M KCl soil extracts.  QuikChem Method 12-107-04-1-B.  Lachat Instruments, Milwaukee, WI.', N'Extractable Nitrate In Botanical Materials - Flow Injection Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (515, N'Plant', N'## Summary
The Total Kjeldahl Nitrogen (TKN) method is based on the wet oxidation of soil organic matter and botanical materials using sulfuric acid and digestion catalyst and conversion of organic nitrogen to the ammonium form. Ammonium is determined using the diffusion-conductivity technique. The procedure does not quantitatively digest nitrogen from heterocyclic compounds (bound in a carbon ring), oxidized forms such as nitrate and nitrite, or ammonium from within mineral lattice structures. The method has a detection limit of approximately 0.001 % N.

Sample amount requested: 3 g

Samples should be powdered and should pass a 1 mm sieve.

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Horneck, D. A. and Miller, R. O. 1998. Determination of Total Nitrogen in Plant Tissue pp. 75-83. In: Kalra, Y. P. (ed.) Handbook of Reference Methods for Plant Analysis, CRC Press, New York.

Isaac, R. A. and Johnson, W. C. 1976. Determination of total nitrogen in plant tissue, using a block digestor. J. Assoc. Off. Anal. Chem. 59:1 98-100.', N'Total Kjeldahl Nitrogen (TKN)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (522, N'Plant', N'## Summary
This analytical method quantitatively determines the total amount of nitrogen and carbon in botanical material using sample combustion coupled with either thermal conductivity/IR detection (LECO FP-528 and TruSpec CN Analyzers) or gas chromatography/thermal conductivity detection (Thermo-Finnigan Flash EA 1112).  The TruSpec CN Analyzer is the preferred instrument, but in the event of very limited sample material the Thermo-Finnigan Flash EA 1112 is used.  The instruments give equivalent test results.  The method is based on the oxidation of the sample by “flash combustion” which converts all organic and inorganic substances into combustion gases (N2, NOx, CO2, and H2O). The method has a detection limit of approximately 0.02% for nitrogen and 0.1% for carbon.

Best results are obtained when the samples appear powder-like. 

Sample amount requested:  3 g (for either or both)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 972.43, Microchemical Determination of Carbon, Hydrogen, and Nitrogen, Automated Method, in Official Methods of Analysis of AOAC International, 18th edition, Revision 1, 2006. Chapter 12, pp. 5-6, AOAC International, Gaithersburg, MD.

AOAC Official Method 990.03. Protein (Crude) in Animal Feed, Combustion Method, in Official Methods of Analysis of AOAC International, 18th Edition (2005).  Revision 1, 2006, Chapter 4, pp. 30-31.  AOAC International, Arlington, VA.', N'Total Nitrogen and Carbon-Combustion Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (530, N'Plant', N'# SO~4~-S
## Summary
This method quantitatively determines the amount of sulfate-sulfur (SO4-S) in botanical materials by extraction with a solution of 2 % acetic acid. Sulfate contained within the extract is bound to an anion exchange resin, organo-sulfur compounds are removed by washing with 0.1N HCl, and sulfate is eluted with 1.0N HCl. The sulfur content of the extract is determined by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES). The method may be semi-quantitative on some botanical materials which have a high anion exchange capacity or in samples where sulfate is not the most significant anionic form of sulfur. The method has a detection limit of 10 ppm.

Sample amount requested:  5 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Littlefield, E. S., Miller, R. O. and Carlson, R. M. 1990. Determination of sulfate-sulfur in plant tissue by inductively coupled plasma spectrometry. Commun. Soil Sci. Plant Anal. 21:1577-1586.', N'Extractable Sulfate-Sulfur In Botanical Materials')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (540, N'Plant', N'# PO~4~-P
## Summary
This method quantitatively determines the amount of phosphorus (PO4-P) in botanical materials by extraction with a solution of 2% acetic acid.  Phosphorus concentration in the extract is determined spectrophotometrically by reacting with ammonium molybdate and antimony potassium tartrate under acidic conditions to form a complex.  This complex is reduced with ascorbic acid to form a blue complex which absorbs light at 880 nm. The absorbance is proportional to the concentration of phosphorus in the sample.  The method has shown to be correlated to plant deficiency and response to phosphorus fertilization.  Samples are analyzed using an automated Flow Injection Analyzer (Lachat). The method has a routine detection limit of 50 ppm but samples that fall below 200 ppm are reanalyzed yielding a detection limit of 20 ppm.

Sample amount requested:  3 g (allows for reporting on a 100% dry basis).

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Prokopy, W. R. 1995. Phosphorus in acetic acid extracts. QuikChem Method 12-115-01-1-C. Lachat Instruments, Milwaukee, WI.', N'Extractable Phosphorus In Botanical Materials')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (550, N'Plant', N'# K
## Summary
This method quantitatively extracts soluble potassium (K) present in botanical materials by extraction with a solution of 2% acetic acid.  Potassium is quantitatively determined in the extract using Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES).  Concentration of potassium is used to assess plant overall nutrient status and is correlated to plant response to potassium fertilization.  The method has a routine detection limit of 0.01% (dry basis).

Sample amount requested: 3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Jones, Jr. J. Benton. 2001. Laboratory Guide for Conducting Soil tests and Plant Analysis. Extraction of Cl, NO~3~, PO~4~, K, and SO~4~ from Plant Tissue. pp. 228-229.

U.S. EPA Method 200.7, Trace Elements in Water, Solids, and Biosolids by Inductively Coupled Plasma-Atomic Emission Spectrometry.', N'Extractable Potassium In Botanical Materials')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (561, N'Plant', N'# Cl by water extraction and analysis by ion chromatography.
## Summary
This method determines the amount of chloride in botanical materials using a water extraction and analysis by ion chromatography with conductivity detection. The method has a detection limit of 0.01 %.

Sample amount requested:  5 g (to allow for moisture determination)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Jones, J. B. 2001. Laboratory Guide for Conducting Soil Tests and Plant Analysis, pp. 227-228.

Dionex Application Note 154, Determination of Inorganic Anions in Environmental Waters Using a Hydroxide-Selective Column.', N'Extractable Chloride In Botanical Materials - Ion Chromatography Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (585, N'Plant', N'# Si
## Summary
This method quantitatively determines the concentration of silicon utilizing a nitric acid/hydrogen peroxide/hydrofluoric acid microwave digestion and analysis by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES). The method has a detection limit of 0.01% and on homogeneous sample material is generally reproducible within 8% (relative).

Sample amount requested:  5 g (to allow for moisture determination)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Feng, X., Wu, S., Wharmby, A., and Wittmeier, A. Microwave digestion of plant and grain standard reference materials in nitric and hydrofluoric acids for multi-elemental determination by inductively coupled plasma mass spectrometry. Journal of Analytical Atomic Spectrometry, 1999, 14, pp. 939-946.', N'Total Silicon-HF')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (590, N'Plant', N'# Plant: B, Ca, Cu, Fe, Mg, Mn, P, K, Na, S, Zn 
[also available: Al, Ba, Cd, Cr, Co, Pb, Mo, Ni, V]
## Summary
This method quantitatively determines the concentration of B, Ca, Cu, Fe, Mg, Mn, P, K, Na, S and Zn and a variety of other elements utilizing a nitric acid/hydrogen peroxide microwave digestion and determination by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES). The methodology utilizes a closed vessel digestion/dissolution of the sample. The method has detection limits ranging from 0.5 ppm to 0.01 %.

|Element  |  MDL|
|----------------|--------------|
|B|1.0 ppm|
|Ca|0.010 %|
|Cu|0.5 ppm|
|Fe|1.0 ppm|
|Mg|0.010 %|
|Mn|1.0 ppm|
|P|0.010 %|
|K|0.01 %|
|Na|1 ppm|
|S|10 ppm|
|Zn|1.0 pp|

<br />
Sample amount requested:   5 g for any or all of the above

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Meyer, G. A. and Keliher, P. N. 1992.  An overview of analysis by inductively coupled plasma-atomic emission spectrometry. pp. 473-516. In: A. Montaser and D.W. Golightly (ed.). Inductively coupled plasmas in analytical atomic spectrometry.  VCH Publishers, New York, NY.

Sah, R. N. and Miller, R. O. 1992. Spontaneous reaction for acid dissolution of biological tissues in closed vessels. Anal. Chem. 64:230-233.', N'Total Elements-Nitric Acid Digestion')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (594, N'Plant', N'# Se
## Summary
This method is quantitative for selenium and is based on the wet oxidation of selenium bearing organic carbon and inorganic selenium compounds utilizing nitric, perchloric and sulfuric acids, reduction of selenate to selenite (IV), and determination by Vapor Generation Inductively-Coupled Plasma Emission Spectrometer (VG-ICP).  The method has a detection limit of 0.05 ppm.

Sample amount requested:  3 g (for reporting on a 100% dry basis)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Tracy, M. L and Moeller, G.  1990.  Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis.  Part 1: Selenium.  J. Assoc. Off. Anal. Chem. 73:404-410.', N'Selenium')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (596, N'Plant', N'# Totals: (Complete Digestion) As
## Summary
This method measures the arsenic concentration in plant tissue using digestion with nitric, perchloric and sulfuric acids, reduction of arsenate to arsenite, and determination by Vapor Generation Inductively-Coupled Plasma Emission Spectrometer (VG-ICP). The method has a detection limit of approximately 0.05 ppm.

Sample amount requested:  3 g  (for reporting on a 100% dry basis)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Tracy, M. L. and Moeller, G. 1990. Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis. Part 2: Arsenic. J. Assoc. Off. Anal. Chem. 74:516-521.', N'Arsenic')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (615, N'Feed', N'# Fat
## Summary
This method quantifies the amount of crude fat (fats, oils, pigments, and other fat soluble substances) in dried forages and mixed feeds using the Randall modification of the standard Soxhlet extraction.  The extraction process includes submerging the test portion into boiling ethyl ether and then lowering the solvent below the sample for a continuous flow of condensed solvent.  The solvent is evaporated and recovered by condensation.  The resulting crude fat residue is determined gravimetrically after drying. The method has a detection limit of 0.25 %.

Samples high in carbohydrates, urea, lactic acid, glycerol, and other water soluble components should undergo water extraction in order to avoid false high values.  Water extraction must be requested by the client at the time of sample submission.

Sample amount required:   15 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 2003.05, Crude Fat in Feeds, Cereal Grains, and Forages, in Official Methods of Analysis of AOAC International, 18th edition (2006), Chapter 4, pp. 40-42, AOAC International, Arlington, VA.
', N'Crude fat')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (625, N'Feed', N'# Protein
## Summary

Total crude protein is calculated from the nitrogen content of the feed material, based on sample type.  The protein factor applied to the nitrogen result is 6.25 unless a different factor is requested.  The method has a detection limit of 0.1% protein (dry basis).

Note:  See SOP 522 for information on nitrogen analysis.

Sample amount requested:  3 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 990.03, Protein (Crude) in Animal Feed, Combustion Method, in Official Methods of Analysis of AOAC International, 18th edition Revision 1, 2006. Chapter 4, pp. 30-31, AOAC International, Gaithersburg, MD.', N'Total Crude Protein In Feed Materials - Combustion Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (640, N'Feed', N'# ADF, ADF (Ash Free), Lignin (Ash Free), ADIN, TDN, Cellulose
## Summary

This procedure is used for the determination of acid detergent fiber (ADF) and ADF-derived tests including lignin, cellulose, TDN (total digestible nutrients) and ADIN (acid detergent insoluble nitrogen) in all types of forages. A hot, acidified detergent solution is used to dissolve cell solubles, hemicellulose and soluble minerals leaving a residue of cellulose, lignin, and heat damaged protein and a portion of cell wall protein and minerals (ash).

Sample amount requested:  20 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

ADF is determined gravimetrically as the residue remaining after acid detergent extraction. Lignin is determined gravimetrically after the ADF residue is extracted with 72% H2SO4 and ashed. Cellulose is determined by subtracting the pre-ash lignin value from the ADF value. TDN is a calculation based on the ADF value and is reported based on the formula for alfalfa. The TDN result is standardized to a 90% dry matter value. ADIN is determined by nitrogen analysis (combustion) of a sub-sample of the ADF residue. (See SOP 525 for information on nitrogen analysis.)

|Analyte                |Typical MDL|
|-------------------------|:------------------:|
|ADF|0.5 %|
|Lignin|1.0 %|
|Cellulose|1.0 %|
|TDN (@90%DM) | 0.5 %|
|ADIN|0.05|

<br />
AOAC Official Method 973.18, Fiber (Acid Detergent) and Lignin in Animal Feed, in Official Methods of Analysis of AOAC International, 16th edition (1997), Chapter 4, pp. 28-29, AOAC International, Arlington, VA.', N'Acid Detergent Fiber (ADF), Lignin (Ash Free), Acid Detergent Insoluble Nitrogen (ADIN), Total Digestible Nutrients (TDN) And Cellulose - Reflux Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (650, N'Feed', N'## Summary
This procedure is used for the determination of amylase-treated neutral detergent fiber (NDF) in feed materials using a neutral detergent solution and heat. Sodium sulfite is used in the procedure to aid in the removal of some nitrogenous matter. Heat-stable amylase is used in the procedure to allow for the removal of starch and to inactivate potential contaminating enzymes that might degrade fibrous constituents. Hemicellulose is determined by performing the NDF procedure followed by the ADF procedure (see SOP 640 for ADF details). The detection limit for NDF and hemicellulose are approximately 0.5 %, and 1.0 % respectively. On homogenous sample material, the method is generally reproducible within 8% (relative). Results for NDF can be reported on an ash-free basis upon client request.

Sample amount requested:  10 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 2002-04, Amylase-Treated Neutral Detergent Fiber in Feeds, Using Refluxing in Beakers or Crucibles, in Official Methods of Analysis of AOAC International, (2006), Chapter 4, pp. 48-55, AOAC International, Arlington, VA.
', N'Neutral Detergent Fiber (NDF) And Hemicellulose - Reflux Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (670, N'Feed', N'# Ash
## Summary
This method quantitatively determines the amount of ash in feed materials based on the gravimetric loss by heating to 550°C for a period of at least three hours. The method has a detection limit of 0.01%.

Sample amount requested: 5 g.

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

AOAC Official Method 942.05, Ash of Animal Feed, in Official Methods of Analysis of AOAC International, 18th edition (2005), Chapter 4, p. 8, AOAC International, Gaithersburg, MD.

NFTA, Section C, Procedure 7, Total Ash in Forages. Printed on July 16, 2009 from www.foragetesting.org.
', N'Ash Content In Botanical Materials - Gravimetric Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (680, N'Feed', N'# Soluble Carbohydrates
## Summary
This method quantitatively determines the amount of the free sugars fructose, glucose and sucrose in botanical materials. Sorbitol content can also be determined upon request. Samples are extracted by hot deionized water and analyzed by HPLC with mass selective detection. The analysis uses a Phenomenex Luna NH2 (250 mm x 4.6 mm) HPLC column at a flow rate of 2.75 mL min acetonitrile:water (78:22). The method has a detection limit of 0.2% and is reproducible within 10% (relative). 

Sample amount requested:  250 mg

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Johansen, H. N., Glitso, V. and Knudsen, K. E. B. 1996. Influence of Extraction Solvent and Temperature on the Quantitative Determination of Oligosaccharides from Plant Materials by High-Performance Liquid Chromatography. J. Agric. Food Chem. 44:1470-1474.
', N'Soluble Carbohydrates - HPLC Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (690, N'Feed', N'# TNC, Starch
## Summary
Nonstructural carbohydrates are those that can be accumulated and then readily mobilized in order to be metabolized or translocated to other plant parts. This method quantitatively determines the amount of the total glucose following enzymatic hydrolysis. Total non-structural carbohydrates (TNC) is the sum of total glucose, free fructose and free sucrose. Starch is the total glucose minus the free glucose multiplied by 0.9. The free carbohydrates are determined by a separate analysis. The samples for total glucose are enzymatically hydrolyzed at 55°C with amyloglucosidase for 12 hours and analyzed by HPLC with mass selective detection. The analysis uses a Phenomenex Luna NH2 (250 mm x 4.6 mm) HPLC column at a flow rate of 2.75 mL min-1 acetonitrile:water (78:22). The method has a detection limit of 0.5% and is reproducible to within 10% (relative).

Smith, Dale. Removing and Analyzing Total Nonstructural Carbohydrates from Plant Tissue. Wisconsin Agric. Exp. Sta. Res. Report 41. 1969.', N'Total Glucose For Total Nonstructural Carbohydrates (TNC) And Starch')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (705, N'Manure and Compost', N'# pH 1:5 pH (Water 1:5)
## Summary
This method determines the pH of the liquid from a slurry of manure, using 1 part sample and 5 parts deionized water. The method is generally reproducible within 0.2 pH units.

Sample amount requested:  30 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Wolf, Nancy. 2003. Determination of manure pH. p. 48-49. In: John Peters (ed.) Recommended Methods of Manure Analysis. University of Wisconsin System.', N'Manure')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (715, N'Manure and Compost', N'# EC (Water 1:5)
## Summary
This method measures the electrical conductivity (ECe) in the liquid from a slurry of manure, using 1 part sample and 5 parts deionized water. The electrical conductivity of the extract is reported. The method has a detection limit of approximately 0.1 dS/m (mmhos/cm).

Sample amount requested:  30 g

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Wolf, Nancy. 2003. Determination of manure electrical conductivity (EC). p. 50-51. In: John Peters (ed.) Recommended Methods of Manure Analysis. University of Wisconsin System.', N'Estimated Soluble Salts (ECe)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (805, N'Water', N'# pH
## Summary
This method determines the pH of water using a pH electrode.  Values are reported to the nearest 0.10 pH unit. 

Sample amount requested:  25 mL

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

 

Method 4500-H+ (pH Value) in Standard Methods for the Examination of Water and Wastewater, 20th Edition.  Clesceri, L. S., Greenberg, A. E. and  Eaton, A. D., eds.; American Public Health Association; Washington D.C., 1998; pp. 4-86 - 4-91.', N'pH')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (810, N'Water', N'# Turbidity
## Summary
Turbidity in water is caused by suspended and colloidal matter and is an expression of the optical property that causes light to be scattered and absorbed rather than transmitted. This method uses a nephelometer to measure turbidity. A nephelometer is a turbidimeter with a scattered-light detector at a 90° angle to the incident beam. Possible interferences with this method include floating debris and coarse sediment which settle out rapidly. Highly colored samples can give artificially low turbidity values. The method has a detection limit of 0.1 NTU.

Clesceri, L. S., A. E. Greenberg and A. D. Eaton. 1998. Method 2130 B. (Turbidity-Nephelometric Method). Standard Methods for the Examination of Water and Wastewater, 20th Edition.', N'Turbidity')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (815, N'Water', N'# EC
## Summary
This method estimates the amounts of soluble salts in the water by measuring the electrical conductivity (EC~e~) of the water sample.  The higher the concentration of salt in a solution, the higher the electrical conductance (the reciprocal of resistance).  Electrical conductivity is a function of quantity and specific types of cations and anions in the water.  The method has a detection limit of 0.01 dS/m.

Sample amount requested:  25 mL

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Method 2510 (Conductivity) in Standard Methods for the Examination of Water and Wastewater 20th Edition. Clesceri, L. S., Greenberg, A. E. and Eaton, A. D., eds. American Public Health Association; Washington D.C.; 1998; pp. 2-24 - 2-47.', N'Estimated Soluble Salts By Electrical Conductivity')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (820, N'Water', N'# Alkalinity, HCO~3~, CO~3~
## Summary
This method measures bicarbonate (HCO~3~^-^), carbonate (CO~3~^2-^) and alkalinity levels in water. Quantitation is by titration with 0.025 N H~2~SO~4~. The method has a routine detection limit of 0.1 meq/L but is capable of a method detection limit for alkalinity of 0.04 meq/L (2 mg CaCO~3~/L) if requested by client.

Sample must be refrigerated.  Sample should not be filtered or acidified.
Sample amount requested:  50 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Method 2320 (Alkalinity) in Standard Methods for the Examination of Water and Wastewater, 20th Edition. Clesceri, L. S., A. E. Greenberg, A. E., and Eaton, A. D., eds.  American Public Health Association; Washington DC; 1998. pp.2-26 - 2-29.', N'Alkalinity, Bicarbonate And Carbonate')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (822, N'Water', N'## Summary
This method quantitatively determines the amount of total carbon and inorganic carbon in liquid samples.  For total carbon, the liquid sample is injected by means of the integrated autosampler into the high temperature combustion reactor with an oxidative catalyst.  In this reactor, at the temperature of 850ºC all organic and inorganic carbon is oxidized into the gaseous carbon dioxide (CO~2~).  The catalyst that is present in the reactor catalyzes the oxidation to completion.  The carbon dioxide is measured at 4.2 um by infrared (IR) detection.  For inorganic carbon, a second injection of the sample is made into the low temperature liquid reactor.  In an acid medium and at room temperature, all inorganic carbon is oxidized to the gaseous carbon dioxide.  The flow of oxygen transports the carbon dioxide to the IR detector to be measured.  The method has a detection limit of 0.5 mg/L.                                        

Sample Amount Requested:  50 mL

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

American Society for Testing and Materials (ASTM International) Standards. Standard Test Method for Total Carbon, Inorganic Carbon, and Organic Carbon in Water by Ultraviolet, Persulfate Oxidation, and Membrane Conductivity Detection.  Designation D 5904 – 02.', N'Total Carbon and Inorganic Carbon')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (830, N'Water', N'# Solubles: Cl
## Summary
This method quantifies the amount of Cl in a water sample. Thiocyanate ion is liberated from mercuric thiocyanate by the formation of soluble mercuric chloride. In the presence of ferric ion, free thiocyanate ion forms the highly colored ferric thiocyanate, of which the absorbance is proportional to the chloride concentration. The absorbance of the ferric thiocyanate is read at 480 nm. The method has a detection limit of 0.1 meq L^-1^ Cl and is generally reproducible within 5%.

Diamond, D. 1994. Determination of Chloride by Flow Injection Analysis Colorimetry. QuikChem Method 10-117-07-1-B. Lachat Instruments, Milwaukee, WI.', N'Chloride - Flow Injection Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (835, N'Water', N'# Solubles: Al, B, Ca, Cd, Cr, Cu, Fe, K, Mg, Mn, Mo, Na, Ni, P, Pb, S, Si, Zn (Other elements are available.  Contact the lab.)
Quantitative determination by ICP-AES.
## Summary

This method quantitatively determines the concentration of the listed elements in water samples by Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES).  Contact the lab if lower detection limits or elements not listed in the table are needed.  Difficult matrices may result in higher MDLs than those listed. 

|Element MDL|(mg/L)|
|-----------|------|
|Al|0.05|
|B|0.01|
|Ca*|0.1|
|Cd|0.005|
|Cr|0.005|
|Cu|0.010|
|Fe|0.010|
|K|0.05|
|Mg*|0.1|
|Mn|0.005|
|Mo|0.005|
|Na*|0.1|
|Ni|0.005|
|P|0.05|
|Pb|0.010|
|S**|0.1|
|Si|0.01|
|Zn|0.005|

<br />

\* Note:  Ca, Mg and Na are reported in meq/L units unless mg/L units are requested on the work request form.  To convert results in meq/L units to mg/L units multiply Ca results by 20, Mg results by 12.15 and Na results by 23.

 ** Note:  Sulfur is reported as SO~4~-S (soluble S) assuming that all sulfur present is in the sulfate form.  If sulfate is of specific interest, the ion chromatographic method (SOP 880) should be requested.

 The EPA recommends that water for soluble analytes be filtered and acidified at the time of collection.   Samples should be filtered through 0.45 uM filters and acidified to pH < 2 with 1+1 HNO~3~ as soon after collection as possible.  If other analyses such as pH, EC, nitrate, etc. are required, a second work request form must be used for the non-treated samples.  Samples submitted without the noted preservation will be tested as received unless in-lab filtration and/or acidification are requested on the work request form.

Sample amount requested:   50 mL  (for any single test and for most combinations of tests)

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

U.S. EPA Method 200.7, Trace Elements in Water, Solids, and Biosolids by Inductively Coupled Plasma-Atomic Emission Spectrometry', N'Soluble Elements')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (837, N'Water', N'# By Special Request
## Summary
This method quantitatively measures the concentration of copper in water samples by Graphite Furnace Atomic Absorption Spectrometry (GFAAS). The method has a detection limit of approximately 0.1 ug/L for copper.

Samples should be refrigerated and acidified until delivery to the lab.
Sample amount requested:  10 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Cabon, J.Y. and Le Bihan, A. 1995. The determination of Cr, Cu and Mn in seawater with transversely heated graphite furnace atomic absorption spectrometry. Spectrochimica Acta Part B 50 (1195) 1703-1716.

Perkin Elmer, The THGA Graphite Furnace Techniques and Recommended Conditions Manual.', N'Copper In Water By Graphite Furnace')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (840, N'Water', N'# SAR, ESP
## Summary
It is possible to estimate the sodium adsorption ratio (SAR) and exchangeable sodium percentage (ESP) after determining Ca, Mg and Na concentrations in water (SOP# 835).  The method has a detection limit of 0.1 for SAR and 1% for ESP.

U.S. Salinity Laboratory Staff. 1954. Choice of determinations and interpretation of data. pp. 25-26. In: L. A. Richards (ed.) Diagnosis and improvement of saline and alkali soils. USDA Agricultural Handbook 60. U.S. Government Printing Office, Washington, D.C.', N'Sodium Adsorption Ratio (SAR) And Exchangeable Sodium Percentage (ESP)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (845, N'Water', N'# NO~3~-N, NH~4~-N
## Summary
This method involves the quantitative analysis of ammonium (NH~4~-N) and nitrate (NO~3~-N) in water. Ammonium and nitrate are measured by the diffusion-conductivity method based on the gaseous diffusing of ammonia (NH~3~) across a gas permeable membrane in the presence of excess base (KOH) and subsequent conductivity detection. Samples can be stored for up to three weeks at low temperature (<4°C). For longer term storage, toluene or thymol should be added to the sample to prevent microbial growth. The method has detection limit of approximately 0.05 mg L^-1^ and is generally reproducible within 7%.

NOTE:  In general, SOP 847 is followed for nitrate and ammonium testing.  (SOP 845 may be used for difficult matrices.)

Carlson, R. M. 1978. Automated separation and conductimetric determination of ammonia and dissolved carbon dioxide. Anal. Chem. 50:1528-1531.', N'Nitrate And Ammonium - Diffusion-Conductivity Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (847, N'Water', N'# NO~3~-N, NH~4~-N, NO~2~-N
## Summary
This method involves the quantitative analysis of ammonium (NH~4~-N), nitrate (NO~3~-N) and nitrite (NO~2~-N) in water. Nitrate is determined by reduction to nitrite via a copperized cadmium column.  The nitrite is then determined by diazotizing with sulfanilamide followed by coupling with N-(1-naphthyl)ethlyenediaminie dihydrochloride.  The absorbance of the product is measured at 520 nm. Nitrite is determined in the same manner with the cadmium column off-line. Ammonia determined by heating with salicylate and hypochlorite in an alkaline phosphate buffer.  The presence of EDTA prevents precipitation of calcium and magnesium.  Sodium nitroprusside is added to enhance sensitivity.  The absorbance of the reaction product is measured at 660 nm and is directly proportional to the original ammonia concentration. The method has detection limits of approximately 0.05 mg/L for all analytes.

Note that the nitrate values reported include any nitrite in the sample. Nitrite is typically an insignificant fraction of the nitrate.

Sample amount requested:   20 mL for Nitrate and/or Ammonium; 20 mL for Nitrite
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Hofer, S. 2003.  Determination of Ammonia (Salicylate) in 2M KCl soil extracts by Flow Injection Analysis.  QuikChem Method 12-107-06-2-A.  Lachat Instruments, Loveland, CO.

Knepel, K. 2003. Determination of Nitrate in 2M KCl soil extracts by Flow Injection Analysis.  QuikChem Method 12-107-04-1-B.  Lachat Instruments, Loveland, CO.

Method 4500-NO3 I. Cadmium Reduction Flow Injection Method (Proposed) in Standard Methods for the Examination of Water and Wastewater, 20th Edition. Clesceri, L. S.; Greenberg, A. E.; and Eaton, A. D.; eds. American Public Health Association; Washington DC; 1998.  pp. 4-121 - 4-122.

Method 4500-NH3 H. Flow Injection Analysis (PROPOSED) in Standard Methods for the Examination of Water and Wastewater, 20th Edition. Clesceri, L. S.; Greenberg, A. E.; and Eaton, A. D.; eds. American Public Health Association; Washington DC; 1998.  pp. 4-111 – 4-112.', N'Nitrate, Nitrite And Ammonium - Flow Injection Analyzer Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (848, N'Water', N'# By Special Request
## Summary
This method involves the quantitation of ammonium (NH~4~-N) in water and waste water. Samples are pH adjusted to pH >9.5, steam distilled into dilute sulfuric acid and analyzed by flow injection analysis (FIA). Ammonium concentration is determined by FIA using the color reaction of NH~4~ with salicylate, nitroprusside and hypochlorite in an alkaline phosphate buffer. The absorbance of the reaction product is measured at 660 nm. The method has a detection limit of approximately 0.5 mg/L and is generally reproducible within 8%.

Sample amount requested:  at least 250 mL
Samples should be frozen or refrigerated until delivered to the lab. Acidification with sulfuric acid to pH < 2 can be used as a preservative.
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

U.S EPA, 1999. Method 1690, Ammonia-N in Water and Biosolids by Automated Colorimetry with Preliminary Distillation.

Switala, K. 1999. Determination of Ammonia by Flow Injection analysis. QuikChem Method 10-107-06-1-A. Lachat Instruments, Milwaukee, WI.', N'Ammonium - Distillation Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (850, N'Water', N'# TKN
## Summary
The Total Kjeldahl Nitrogen (TKN) method is based on the wet oxidation of nitrogen using sulfuric acid and digestion catalyst.  The procedure converts organic nitrogen to the ammonium form.  Ammonium is subsequently quantitated by the diffusion-conductivity technique.  The procedure does not quantitatively digest nitrogen from heterocyclic forms (bound in a carbon ring) or from oxidized forms such as nitrate and nitrite.  The method has a detection limit of approximately 0.1mg/L N. 

Sample amount requested:  50 mL
Samples should be kept under refrigeration until analysis can be completed.
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Carlson, R. M. 1986. Continuous flow reduction of nitrate to ammonia with granular zinc.  Anal. Chem. 58:1590-1591.', N'Total Kjeldahl Nitrogen - TKN')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (855, N'Water', N'## Summary
This method quantitatively determines the total amount of nitrogen in liquid samples. The sample is injected by an autosampler into a high temperature (850ºC) combustion reactor with an oxidative catalyst, converting all forms of nitrogen to nitric oxide (NO). The NO is quantitated with a chemiluminescent detector. Samples should be homogeneous with particles smaller than 0.45 um. The method has a detection limit of 0.1 mg/L.

Sample amount requested:  50 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Sample storage: samples should be stored at less than 6ºC.

American Society for Testing and Materials (ASTM International) Standards. Standard Test Method for Total Chemically Bound Nitrogen in Water by Pyrolysis and Chemiluminescence Detection. Designation D 5176 – 91 (Reapproved 2003).', N'Total Nitrogen - Combustion Method')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (865, N'Water', N'# PO~4~-P
## Summary
This method quantitatively determines the amount of PO~4~-P in water. Orthophosphate concentration in water is determined spectrophotometrically by reacting with ammonium molybdate and antimony potassium tartrate under acidic conditions to form a complex. This complex is reduced with ascorbic acid to form a blue complex which absorbs light at 880 nm. The absorbance is proportional to the concentration of phosphorus in the sample. Samples are analyzed using an automated Flow Injection Analyzer (Lachat). The method has a detection limit of 0.05 mg/L.

Sample amount requested:  10 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Method 4500-P G (Flow Injection Analysis for Orthophosphate) in Standard Methods for the Examination of Water and Wastewater, 20th Edition. Clesceri, L. S.; Greenberg, A. E.; and Eaton, A. D.; eds. American Public Health Association; Washington DC; 1998. pp. 4-149 - 4-150.', N'Soluble Phosphorous In Water')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (870, N'Water', N'## TS, TSS, TDS, VSS
# Summary
This method quantifies solids in water or wastewater samples using gravimetric analysis following oven drying.   Solids refer to matter suspended or dissolved in the water or wastewater and may affect water or effluent quality in adverse ways. Waters with high dissolved solids generally are of inferior palatability and may induce unfavorable physiological reactions in transient consumers. Solids analyses are important in the control of biological and physical wastewater treatment processes and for assessing compliance with regulatory agency limitations. The method has a detection limit of approximately 4 mg/L for TSS and 10 mg/L for TDS, TS and VSS. The results are generally reproducible within 10%. 
Sample should be kept under refrigeration until analysis can be completed.

Sample amount requested for TS:  500 mL
Sample amount requested for TDS and/or TSS and/or VSS: 500 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Method 2540 B. (Total Solids Dried at 103-105ºC), Method 2540 C. (Total Dissolved Solids Dried at 180ºC), Method 2540 D. (Total Suspended Solids Dried at 103-105ºC) and Method 2540 E. (Fixed and Volatile Solids Ignited at 550ºC) in Standard Methods for the Examination of Water and Wastewater, 20th Edition. Clesceri, L. S.; Greenberg, A. E.; and Eaton, A. D.; eds. American Public Health Association; WashingtonDC; 1998. pp. 2-55 – 59.', N'Total Solids, Total Suspended Solids, Total Dissolved Solids, Volatile Suspended Solids')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (875, N'Water', N'## Hardness
# Summary
It is possible to calculate the hardness of a water sample after determining Ca and Mg concentrations in water (SOP# 835). The method has a detection limit of 1 mg/L as CaCO~3~.

Method 314A (Hardness by Calculation) in Standard Methods for the Examination of Water and Wastewater, 16th Edition. Greenberg, A. E.; Trussell, R. R.; Clesceri, L. S.; eds. American Public Health Association; Washington DC; 1985.  p. 209.', N'Hardness')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (880, N'Water', N'## SO~4~, CL
# Summary
This method quantitatively measures the concentration of sulfate and chloride in water samples by Ion Chromatography (IC). The method has detection limits of approximately 0.5 mg/L for sulfate and chloride.

Sample amount requested:  5 mL
Samples should be refrigerated until delivery to the lab.
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

U.S. EPA Method 300.0, Determination of Inorganic Anions by Ion Chromatography, Revision 2.1, 1993.', N'Anions By Ion Chromatography')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (890, N'Water', N'## Water Totals: P, K, S, Ca, Mg, Na, B, Zn, Mn, Fe, Cu, Al, Cd, Cr, Pb, Mo, Ni
# Summary
This method quantitatively determines the concentration of P, S, Ca, Mg, K, Na, B, Zn, Mn, Fe, Cu, Mo and a variety of other elements utilizing a nitric acid/hydrogen peroxide microwave digestion and determination by atomic absorption spectrometry (AAS) and Inductively Coupled Plasma Atomic Emission Spectrometry (ICP-AES). The methodology utilizes a pressure digestion/dissolution of the sample and is incomplete relative to the total oxidation of organic carbon. K, Na, Zn, Cu, Mn, and Fe are analyzed by AAS and all others are analyzed by ICP-AES with vacuum spectrometer. The method has detection limits ranging from 0.1 mg/Kg to 0.01%. The method is generally reproducible within 8% for all analytes.

Sah, R. N. and Miller, R. O. 1992. Spontaneous reaction for acid dissolution of biological tissues in closed vessels. Anal. Chem. 64:230-233.

Meyer, G. A. and Keliher, P. N. 1992. An overview of analysis by inductively coupled plasma-atomic emission spectrometry. p. 473-516. In: A. Montaser and D.W. Golightly (ed.) Inductively coupled plasmas in analytical atomic spectrometry. VCH Publishers Inc. New York, NY.', N'Total Elements (Includes Phosphorus, Sulfur, Potassium, Calcium, Magnesium, Sodium, Boron, Zinc, Manganese, Iron, Copper And Molybdenum)')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (891, N'Water', N'## By Special Request
# Summary
This method quantitatively measures the concentration of total copper in water samples by Graphite Furnace Atomic Absorption Spectrometry (GFAAS). The method has a detection limit of approximately 0.5 ug/L for copper.

Sample amount requested:  100 mL
Samples should be acidified and refrigerated until delivery to the lab.
Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Cabon, J.Y. and Le Bihan, A. 1995. The determination of Cr, Cu and Mn in seawater with transversely heated graphite furnace atomic absorption spectrometry. Spectrochimica Acta Part B 50 (1195) 1703-1716.

U.S. EPA Method 200.7, Trace Elements in Water, Solids, and Biosolids by Inductively Coupled Plasma-Atomic Emission Spectrometry.', N'Total Copper In Water By Graphite Furnace')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (894, N'Water', N'## Totals: Se
# Summary
This method is quantitative for selenium. Samples are digested using perchloric and sulfuric acids and reduced using hydrochloric acid (selenate to selenite (IV)).  Determination is by vapor generation Inductively-Coupled Plasma Emission Spectrometer (ICP-AES). The method has a detection limit of 0.5 ug/L.

Sample amount requested:  40 mL

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

Tracy, M. L. and Moeller, G. 1990. Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis. Part 1: Selenium. J. Assoc. Off. Anal. Chem. 73:404-410.', N'Selenium')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (896, N'Water', N'## Totals: (Complete Digestion) As
# Summary
This method is quantitative for arsenic. Samples are digested using perchloric and sulfuric acids and reduced using hydrochloric acid (arsenate to arsenite).  Determination is by vapor generation Inductively-Coupled Plasma Emission Spectrometer (ICP-AES). The method has a detection limit of 0.5 ug/L.

Sample amount requested:  40 mL

Questions concerning limited sample size can be answered by the UC Davis Analytical Laboratory.

 

Tracy, M. L. and Moeller, G. 1990. Continuous flow vapor generation for inductively coupled argon plasma spectrometric analysis. Part 2: Arsenic. J. Assoc. Off. Anal. Chem. 74:516-521.', N'Arsenic')
GO
INSERT [dbo].[AnalysisMethods] ([Id], [Category], [Content], [Title]) VALUES (898, N'Water', N'## Totals: Hg
# Summary
This method is quantitative for mercury utilizing nitric acid and potassium permanganate digestion, and determination by Vapor Generation Inductively-Coupled Plasma Emission Spectrometer (VG-ICP). The method has a detection limit of approximately 2 ug/L.

Sample amount requested:  30 mL
Questions concerning limited sample size can be answered by the UC Davis Analytical laboratory.

Melton, Larry.  January 26, 2000. Mercury Quantitation by Hydride Generation ICP. CAHFS Toxicology Laboratory Standard Operating Procedure. HGVICP ver 04.', N'Mercury')
GO
