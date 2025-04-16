# CAS API Gateway

A collection of CAS APIs

# DOCKER
`docker build . -t cas-api`
`docker run --rm -p 8080:8080 --name cas-api cas-api`

# RESOURCES
Contact as of April 15, 2025: Adrien French

Proxies
https://wsgw.test.jag.gov.bc.ca/victim/ords/cas/ (TEST)
https://wsgw.test.jag.gov.bc.ca/victim/ords/castrain/ (TRAINING)
https://wsgw.jag.gov.bc.ca/victim/ords/cas/ (PROD)


Policies
https://wsgw.test.jag.gov.bc.ca/victim/api/cas/api/CASAPTransaction routes to
https://cas-interface-service-f9a9e6-test.apps.silver.devops.gov.bc.ca
 
https://wsgw.test.jag.gov.bc.ca/victim/ords/cas/ routes to:
https://cfs-systws.cas.gov.bc.ca:7025/ords/cas/oauth/token
 
https://wsgw.test.jag.gov.bc.ca/victim/ords/castrain/ routes to:
https://cfs-systws.cas.gov.bc.ca:7026/ords/cas/cfs/apinvoice
 
https://wsgw.jag.gov.bc.ca/victim/ords/cas/ routes to:
https://cfs-prodws.cas.gov.bc.ca:7121/ords/cas/oauth/token

https://wsgw.jag.gov.bc.ca/victim/ords/cas/cfs/apinvoice/* routes to:
https://cfs-prodws.cas.gov.bc.ca:7121/ords/cas/cfs/apinvoice
