import {
  getSystemStatus,
  getSystemStatusQueryKey,
  vGetSystemStatusResponse,
} from '@music-player/api-client'
import { queryOptions } from '@tanstack/react-query'
import * as v from 'valibot'

export const systemStatusQuery = queryOptions({
  queryKey: getSystemStatusQueryKey(),
  queryFn: async ({ signal }) => {
    const { data } = await getSystemStatus({ signal, throwOnError: true })

    return v.parse(vGetSystemStatusResponse, data)
  },
  staleTime: 30_000,
})
