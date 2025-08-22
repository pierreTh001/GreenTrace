import { Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import ProtectedRoute from './components/ProtectedRoute'
import Shell from './components/Shell'

import Login from './pages/auth/Login'
import Register from './pages/auth/Register'

import Dashboard from './pages/Dashboard'
import CompanyProfile from './pages/CompanyProfile'
import Materiality from './pages/Materiality'
import DataCollection from './pages/DataCollection'
import Emissions from './pages/Emissions'
import Users from './pages/Users'
import Settings from './pages/Settings'
import ReportPreview from './pages/ReportPreview'
import ComplianceChecklist from './pages/ComplianceChecklist'
import AuditTrail from './pages/AuditTrail'

export default function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/" element={<Navigate to="/app" />} />
        <Route path="/auth/login" element={<Login />} />
        <Route path="/auth/register" element={<Register />} />
        <Route element={<ProtectedRoute><Shell /></ProtectedRoute>}>
          <Route path="/app" element={<Dashboard />} />
          <Route path="/app/company" element={<CompanyProfile />} />
          <Route path="/app/materiality" element={<Materiality />} />
          <Route path="/app/data" element={<DataCollection />} />
          <Route path="/app/emissions" element={<Emissions />} />
          <Route path="/app/report" element={<ReportPreview />} />
          <Route path="/app/users" element={<Users />} />
          <Route path="/app/settings" element={<Settings />} />
          <Route path="/app/compliance" element={<ComplianceChecklist />} />
          <Route path="/app/audit" element={<AuditTrail />} />
        </Route>
        <Route path="*" element={<Navigate to="/auth/login" />} />
      </Routes>
    </AuthProvider>
  )
}
