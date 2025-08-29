import { createContext, useContext, useEffect, useState } from 'react'
import { AuthService, type User } from '../services/AuthService'

type AuthContextType = {
  user: User | null
  login: (email: string, password: string) => Promise<boolean>
  register: (email: string, password: string, firstName: string, lastName: string) => Promise<boolean>
  logout: () => void
}

const AuthContext = createContext<AuthContextType>({
  user: null,
  login: async () => false,
  register: async () => false,
  logout: () => {}
})

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(AuthService.currentUser())

  useEffect(() => {
    const unsub = AuthService.subscribe(setUser)
    return () => unsub()
  }, [])

  return (
    <AuthContext.Provider value={{
      user,
      login: async (e,p) => AuthService.login(e,p),
      register: async (e,p,fn,ln) => AuthService.register(e,p,fn,ln),
      logout: () => AuthService.logout()
    }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
