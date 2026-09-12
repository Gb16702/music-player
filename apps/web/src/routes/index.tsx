import { createFileRoute } from '@tanstack/react-router'
import { useSuspenseQuery } from '@tanstack/react-query'
import { systemStatusQuery } from '../queries/system-status'

export const Route = createFileRoute('/')({
  loader: ({ context }) => context.queryClient.ensureQueryData(systemStatusQuery),
  component: Home,
})

function Home() {
  const { data } = useSuspenseQuery(systemStatusQuery)

  return (
    <main className="grid min-h-screen place-items-center bg-[#0b0b0d] px-6 text-white">
      <section className="w-full max-w-xl rounded-3xl border border-white/10 bg-white/5 p-8 shadow-2xl shadow-black/40 backdrop-blur">
        <p className="mb-3 text-xs font-semibold uppercase tracking-[0.24em] text-rose-400">
          Music Player
        </p>
        <h1 className="text-4xl font-semibold tracking-tight">
          Le socle est prêt.
        </h1>
        <p className="mt-4 leading-7 text-white/60">
          TanStack Start est connecté à notre API .NET via un contrat OpenAPI
          généré et validé par Valibot.
        </p>
        <div className="mt-8 flex items-center gap-3 text-sm text-white/70">
          <span
            aria-hidden="true"
            className="size-2.5 rounded-full bg-emerald-400 shadow-[0_0_18px_rgba(52,211,153,0.8)]"
          />
          API {data.status}
        </div>
      </section>
    </main>
  )
}
