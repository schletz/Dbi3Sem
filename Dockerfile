# docker build -t workspacemanager .
# Variante 1
# docker run -d -p 80:80 --name workspacemanager workspacemanager
# Variante 2 (Volume)
# docker run -d -p 80:80 -v C:\Users\Michael\Downloads\workspacemanager\www:/var/www/html --name workspacemanager workspacemanager

FROM ubuntu:22.04
RUN ln -snf /usr/share/zoneinfo/UTC /etc/localtime
RUN echo UTC > /etc/timezone
RUN apt-get update && apt-get -y install php8.1

# Variante 1: Copy application (for deployment)
# RUN rm -rf /var/www/html/*
# COPY www/* /var/www/html

# Variante 2: Source as volume
VOLUME /var/www/html

EXPOSE 80 443
CMD apachectl -D FOREGROUND
