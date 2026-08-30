    dotnet publish -c Release -o ./bin/Publish

    mrsimi

     az webapp deploy --resource-group "TurntableChartsRG" --name "turntablecharts-api" --src-path ./deploy.zip --type zip