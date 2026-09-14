import { defineConfig } from '@hey-api/openapi-ts'

export default defineConfig({
  input: 'http://127.0.0.1:5100/openapi/v1.json',
  output: {
    path: 'packages/api-client/src/generated',
    clean: true,
  },
  plugins: [
    '@hey-api/client-fetch',
    '@hey-api/typescript',
    '@hey-api/sdk',
    'valibot',
    '@tanstack/react-query',
  ],
})
