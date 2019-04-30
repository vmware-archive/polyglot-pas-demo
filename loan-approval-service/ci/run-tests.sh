#!/usr/bin/env bash

set -e

if [ -n $checkerURL ]
then
    export checkerURL=http://loan-checker-test.apps.cloudyazure.io
    echo -e "Setting URL for the Loan Checker to ${checkerURL}"
fi

env URL="$checkerURL" ./tasks/smoke-test-loan-checker.sh