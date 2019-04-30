#!/usr/bin/env bash

set -e +x

export FOLDER=`pwd`
echo "The path is ${OLDPATH}"

echo "Testing and Packaging the Loan Checker JAR..."
cd source-code/loan-approval-service
  mvn verify -P cloud -Dspring.profiles.active=simcloud
cd $FOLDER

jar_count=`find source-code/loan-approval-service/target -type f -name *.jar | wc -l`

if [ $jar_count -gt 1 ]; then
  echo "More than one JAR was found. I don't know which one to work with. Exiting :("
  exit 1
fi

find source-code/loan-approval-service/target -type f -name *.jar -exec cp "{}" package-output/loan-approval-service.jar \;

echo "Done packaging"
exit 0