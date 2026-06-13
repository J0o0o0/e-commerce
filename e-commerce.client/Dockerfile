# STAGE 1: Build Angular with Node.js
FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .

# Fix Angular budget limits
RUN sed -i 's/"maximumError": "[^"]*"/"maximumError": "100mb"/g' angular.json
RUN sed -i 's/"maximumWarning": "[^"]*"/"maximumWarning": "100mb"/g' angular.json

# Build Angular
RUN npx ng build --output-path=/app/dist/out

# STAGE 2: Serve with Nginx
FROM nginx:alpine

# Remove ALL default Nginx files (including the welcome page)
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular files from the browser subfolder directly into html root
COPY --from=build /app/dist/out/browser /usr/share/nginx/html

# Copy Nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
