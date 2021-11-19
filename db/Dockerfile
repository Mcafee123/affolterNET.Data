FROM mcr.microsoft.com/mssql/server:2019-CU13-ubuntu-20.04

ARG DATABASE_PASSWORD

ENV SA_PASSWORD ${DATABASE_PASSWORD}
ENV ACCEPT_EULA "Y"

WORKDIR /usr/src/app
COPY . /usr/src/app

RUN echo "${DATABASE_PASSWORD}" > /tmp/_pw.sh

CMD ./entrypoint.sh