# .NET25 laboration: Generativ AI

En enkel webbchatt där du kan skicka meddelanden till en AI-modell via Azure AI Foundry.

## Kom igång

Du behöver .NET 10 SDK och ett Azure AI Foundry-projekt med en tillgänglig `gpt-5.4-mini`-deployment.

1. Klona repot och öppna projektmappen i en terminal.
2. Skapa en fil med namnet `.env` i projektets rotmapp. Lägg in projektets endpoint och API-nyckel:

	```env
	FOUNDRY_ENDPOINT=https://<ditt-projekt>.services.ai.azure.com/api/projects/<ditt-projekt>
	FOUNDRY_API_KEY=<din-api-nyckel>
	```

	Ersätt platshållarna med uppgifterna från ditt Azure AI Foundry-projekt. `.env` är lokal konfiguration och ignoreras av Git.

3. Starta appen:

	```powershell
	dotnet run
	```

4. Öppna adressen som visas i terminalen. Med projektets vanliga HTTP-profil är adressen `http://localhost:5273`.

## Så fungerar programmet

Webbsidan i `wwwroot` låter användaren skriva ett meddelande och skickar det som JSON till appens `POST /api/chat`-endpoint. ASP.NET Core kontrollerar att meddelandet innehåller 1–4000 tecken. Servern skickar sedan meddelandet till AI-modellen och returnerar svaret till webbsidan, som visar det i chatten. Varje anrop innehåller bara det aktuella meddelandet. Tidigare chattmeddelanden skickas inte med som kontext.

## AI-tjänst

Projektet använder Azure AI Foundry med Azure OpenAI Responses API och deploymenten `gpt-5.4-mini`. Foundry ger projektet en hanterad endpoint och autentisering för modellåtkomst. Mini-modellen passar den här enkla chattdemonstrationen, där målet är att få AI-svar utan att integrera en större modell än uppgiften kräver. Endpoint och nyckel läses från miljövariabler så att hemligheter inte behöver lagras i källkoden.
