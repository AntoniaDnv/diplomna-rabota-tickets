# Stage 1: build. Node is needed only to turn the React source into plain HTML, JS and CSS.
FROM node:22-alpine AS build
WORKDIR /app

# The package files are copied first: if they have not changed,
# Docker reuses the cached "npm ci" layer and the build is much faster.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build
# The result is in /app/dist

# Stage 2: run. The final image has only nginx and the built files - no Node, no source code.
FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
