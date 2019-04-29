#!/usr/bin/env bash

apt-get update && apt-get install -y curl uuid-runtime httpie jq --allow-unauthenticated

set -eu

echo "Smoke Testing the Loan Checker using the URL: ${URL}"

# Begin the Smoke-testing...

export HEALTH_STATUS=`curl -sL -X GET ${URL}/actuator/health | jq -r .status`
if [ -z $HEALTH_STATUS ] || [ "$HEALTH_STATUS" != "UP" ]
then
    echo -e "Actuator /health check has failed!"
    exit 1
else
    echo "Actuator health check status is reporting the app is ${HEALTH_STATUS}"
fi

# Make sure the /actuator/info endpoint shows...

if curl -sL -w %{http_code} "$URL/actuator/info" -o /dev/null | grep "200"
then
    echo "Actuator is showing /info correctly."
else
    echo -e "Error. Actuator is not showing '200 OK' on [$URL/actuator/info]"
    exit 1
fi

# Make sure the /actuator/env endpoint shows...
if curl -sL -w %{http_code} "$URL/actuator/env" -o /dev/null | grep "200"
then
    echo "Actuator is showing /env correctly."
else
    echo -e "Error. Actuator is not showing '200 OK' on [$URL/actuator/env]"
    exit 1
fi

# Test the Loan Checker API...

export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Wilcock amount=100 status=PENDING`
# echo -e ${LOAN_CHECK_RESP}
# echo ${LOAN_CHECK_RESP} | jq -r .status
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "APPROVED" ]]
then
    echo -e "Test failed. Loan was not approved ($LOAN_CHECK_RESP)"
    exit 1
else
    echo -e "Test passed. Loan is APPROVED"
fi

# Test that loans for people on the Naughty List are REJECTED
export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Trunp amount=100 status=PENDING`
echo -e ${LOAN_CHECK_RESP}
# echo ${LOAN_CHECK_RESP} | jq -r .status
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "REJECTED" ]]
then
    echo -e "Test failed. Loan was approved ($LOAN_CHECK_RESP)"
    exit 1
else
    echo -e "Test passed. Naughty person's loan is REJECTED"
fi

# Test that loans over the Max Amount Threshold are REJECTED
export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Wilcock amount=100000000 status=PENDING`
echo -e ${LOAN_CHECK_RESP}
# echo ${LOAN_CHECK_RESP} | jq -r .status
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "REJECTED" ]]
then
    echo -e "Test failed. Loan was approved ($LOAN_CHECK_RESP)"
    exit 1
else
    echo -e "Test passed. Large loan is REJECTED"
fi

echo "*************** SUCCESS! ****************"
echo "The LOAN-CHECKER SMOKE TEST has finished."
echo "             ZERO (0) ERRORS!"
echo "  Nice work, you didn't break anything!"
echo "*****************************************"
exit 0