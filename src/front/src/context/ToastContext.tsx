import React, { createContext, useContext, useMemo, useState } from 'react'

type ToastType = 'success' | 'warning' | 'error'
type Toast = { id: string; type: ToastType; message: string }

type ToastContextType = {
  success: (msg: string) => void
  warning: (msg: string) => void
  error: (msg: string) => void
}

const ToastContext = createContext<ToastContextType | null>(null)

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])

  function push(type: ToastType, message: string, ttlMs = 4000) {
    const id = crypto.randomUUID()
    setToasts((t) => [...t, { id, type, message }])
    window.setTimeout(() => {
      setToasts((t) => t.filter((x) => x.id !== id))
    }, ttlMs)
  }

  const api = useMemo<ToastContextType>(() => ({
    success: (m) => push('success', m),
    warning: (m) => push('warning', m),
    error: (m) => push('error', m)
  }), [])

  return (
    <ToastContext.Provider value={api}>
      {children}
      <div className="fixed bottom-4 right-4 z-[1000] space-y-2">
        {toasts.map(t => (
          <div key={t.id}
            className={
              `min-w-[260px] max-w-sm rounded-xl px-4 py-3 shadow-lg text-sm text-white flex items-start gap-3 ${
                t.type === 'success' ? 'bg-emerald-600' : t.type === 'warning' ? 'bg-amber-600' : 'bg-red-600'
              }`}
          >
            <div className="flex-1">{t.message}</div>
            <button
              aria-label="Fermer"
              className="text-white/80 hover:text-white"
              onClick={() => setToasts((x) => x.filter((y) => y.id !== t.id))}
            >×</button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast() {
  const ctx = useContext(ToastContext)
  if (!ctx) throw new Error('useToast must be used within ToastProvider')
  return ctx
}

