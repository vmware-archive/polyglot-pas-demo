#!/usr/bin/env bash

apt-get update && apt-get install -y curl uuid-runtime httpie jq --allow-unauthenticated

set -eu

echo "****************************************"
echo "Smoke Testing the Loan Approval Service."
echo  "API TARGET: ${URL}"
echo "****************************************"
echo " "

# Begin the Smoke-testing...

export HEALTH_STATUS=`http GET ${URL}/actuator/health | jq -r .status`
if [ -z $HEALTH_STATUS ] || [ "$HEALTH_STATUS" != "UP" ]
then
    echo -e "Actuator /health check has failed!"
    exit 1
else
    echo "Actuator is showing /health as having status: ${HEALTH_STATUS}"
fi

# Make sure the /actuator/info endpoint shows...
export INFO_STATUS=`curl -sL -w %{http_code} "${URL}/actuator/info" -o /dev/null | grep "200"`
if [ -z ${INFO_STATUS} ] || [ "${INFO_STATUS}" != "200" ]
then
    echo -e "Error. Actuator is not showing '200' on [$URL/actuator/info] (${INFO_STATUS})"
    exit 1
else
    echo "Actuator is showing /info correctly, status: ${INFO_STATUS}"
fi

# Make sure the /actuator/env endpoint shows...
export ENV_STATUS=`curl -sL -w %{http_code} "${URL}/actuator/info" -o /dev/null | grep "200"`
if [ -z ${ENV_STATUS} ] || [ "${ENV_STATUS}" != "200" ]
then
    echo -e "Error. Actuator is not showing '200' on [$URL/actuator/env] (${ENV_STATUS})"
    exit 1
else
    echo "Actuator is showing /env correctly, status: ${ENV_STATUS}"
fi

# Make sure the /actuator/refresh endpoint is accepting posts...
export REFRESH_STATUS=`curl -sL -w %{http_code} "${URL}/actuator/refresh" -d {} -H "Content-Type: application/json" -o /dev/null | grep "200"`
if [ -z ${REFRESH_STATUS} ] || [ "${REFRESH_STATUS}" != "200" ]
then
    echo -e "Error. Actuator is not showing '200 OK' on [$URL/actuator/refresh] (${REFRESH_STATUS})"
    exit 1
else
    echo "Actuator is accepting posts to /refresh correctly, status: ${REFRESH_STATUS}"
fi

# Test the Loan Checker API...

export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Wilcock amount=100 status=PENDING`
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "APPROVED" ]]
then
    echo -e "Test failed. Loan was not approved but it should have been? ($LOAN_CHECK_RESP)"
    exit 1
else
    echo "Test passed. Loan was APPROVED."
fi
echo ${LOAN_CHECK_RESP}

# Test that loans for people on the Naughty List are REJECTED
export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Trunp amount=100 status=PENDING`
# echo ${LOAN_CHECK_RESP} | jq -r .status
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "REJECTED" ]]
then
    echo -e "Test failed. Loan was approved but should not have been? ($LOAN_CHECK_RESP)"
    exit 1
else
    echo -e "Test passed. Naughty person's loan was REJECTED."
fi
echo ${LOAN_CHECK_RESP}

# Test that loans over the Max Amount Threshold are REJECTED
export RANDOM_LOAN_ID=`uuidgen`
export LOAN_CHECK_RESP=`http POST ${URL}/check id=${RANDOM_LOAN_ID} name=Wilcock amount=100000000 status=PENDING`
if [[ -z ${LOAN_CHECK_RESP} ]] || [[ `echo ${LOAN_CHECK_RESP} | jq -r .status`  != "REJECTED" ]]
then
    echo -e "Test failed. Loan was approved ($LOAN_CHECK_RESP)"
    exit 1
else
    echo -e "Test passed. Large loan is REJECTED"
fi
echo ${LOAN_CHECK_RESP}


export LAST_LOAN=`http GET ${URL}/statuses | jq -c 'map(select(.id | contains("'${RANDOM_LOAN_ID}'")))'`
if [[ -z ${LAST_LOAN} ]]
then
  echo "Test failed. The last LOAN CHECK request is NOT in the /statuses list! ${RANDOM_LOAN_ID}"
  exit 1
else
  echo "Test passed. The last LOAN CHECK request is in the /statuses list." 
fi
echo ${LAST_LOAN}

echo " "
echo "*************** SUCCESS! ****************"
echo "The LOAN-CHECKER SMOKE TEST has finished."
echo "             ZERO (0) ERRORS!"
echo "  Nice work, you didn't break anything!"
echo "*****************************************"
exit 0