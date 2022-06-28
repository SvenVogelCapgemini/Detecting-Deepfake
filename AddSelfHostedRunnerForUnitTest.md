#Zelfgehoste github runner toe aan dit project
Voor dit project heb je een een zelfgehoste github-runner nodig voor het uitvoeren van de unittests.
Dit komt door dat python nodig is om deze uit te voeren. 
om dit te doen heb je eerst een computer nodig die je als runner wilt gebruiken.
Installeer python en alle juiste requirement op dezelfde manier als de workerSetup in de Nodes Setup document.
ook is een .net 6 sdk nodig op deze computer om de unit tests uit te voeren.
als dat allemaal geinstaleerd is ben je klaar om de runner toe te voegen.

Stap 1: ga naar de runner page van de repo en klik op new self hosted runner
![github runner page](https://i.imgur.com/QvOB0Bw.png)

Stap 2: Selecteer de juiste OS en architecture (Windows x64 in mijn geval)
en vlog de stappen die er onder staan
![github runner page](https://i.imgur.com/QvOB0Bw.png)
