docker build . -t buk-gg
docker tag buk-gg eu.gcr.io/buk-gg/bukgaming:latest
docker push eu.gcr.io/buk-gg/bukgaming:latest