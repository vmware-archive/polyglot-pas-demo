$ErrorActionPreference = "Stop"

cf create-service p-config-server standard loans-config -c .\scripts\config-server.json
cf create-service p-service-registry standard loans-discovery
cf create-service azure-sqldb basic loans-sql
#cf create-service p-redis shared-vm loan-app-sessions
#cf create-service p-circuit-breaker-dashboard standard loan-app-hystrix
#cf cups myOAuthService -p "{\"client_id\": \"dave1App\",\"client_secret\": \"dave1Secret\",\"uri\": \"uaa://login.sys.selma.cf-app.com\"}"

cf share-service loans-config -o loan-checker -s prod
cf share-service loans-discovery -o loan-checker -s prod

cf share-service loans-config -o loan-checker -s test
cf share-service loans-discovery -o loan-checker -s test