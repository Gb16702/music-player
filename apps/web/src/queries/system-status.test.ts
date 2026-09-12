import { vGetSystemStatusResponse } from '@music-player/api-client'
import { describe, expect, it } from 'vitest'
import * as v from 'valibot'

describe('system status contract', () => {
  it('accepts the API payload shape', () => {
    const result = v.safeParse(vGetSystemStatusResponse, {
      status: 'ok',
      timestamp: '2026-08-29T03:38:22.963Z',
    })

    expect(result.success).toBe(true)
  })

  it('rejects an invalid cross-boundary payload', () => {
    const result = v.safeParse(vGetSystemStatusResponse, {
      status: 200,
      timestamp: 'not-a-timestamp',
    })

    expect(result.success).toBe(false)
  })
})
