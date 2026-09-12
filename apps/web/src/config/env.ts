import * as v from 'valibot'

const PublicEnvironmentSchema = v.object({
  VITE_API_BASE_URL: v.optional(
    v.pipe(v.string(), v.url()),
    'http://localhost:5100',
  ),
})

export const publicEnvironment = v.parse(
  PublicEnvironmentSchema,
  import.meta.env,
)
