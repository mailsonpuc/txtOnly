import { BrowserRouter, Navigate, Outlet, Route, Routes, useParams } from 'react-router-dom'
import { useAuth } from './auth/AuthContext'
import { AuthProvider } from './auth/AuthProvider'
import { LoadingLine, TerminalShell } from './components/TerminalShell'
import { AuthPage } from './pages/AuthPage'
import { FeedPage } from './pages/FeedPage'
import { PeoplePage } from './pages/PeoplePage'
import { ProfilePage } from './pages/ProfilePage'
import './social.css'

function ProtectedRoutes() {
  const { user, loading } = useAuth()
  if (loading) return <main className="auth-screen"><LoadingLine>restaurando sessão...</LoadingLine></main>
  return user ? <Outlet /> : <Navigate to="/login" replace />
}

function LegacyProfileRedirect() {
  const { userId } = useParams()
  return <Navigate to={userId ? `/profiles/${userId}` : '/profiles'} replace />
}

function AppRoutes() {
  return <Routes>
    <Route path="/login" element={<AuthPage mode="login" />} />
    <Route path="/register" element={<AuthPage mode="register" />} />
    <Route element={<ProtectedRoutes />}>
      <Route element={<TerminalShell />}>
        <Route path="/" element={<Navigate to="/feed" replace />} />
        <Route path="/feed" element={<FeedPage />} />
        <Route path="/users" element={<PeoplePage />} />
        <Route path="/profiles" element={<ProfilePage own />} />
        <Route path="/profiles/:userId" element={<ProfilePage />} />
        <Route path="/people" element={<Navigate to="/users" replace />} />
        <Route path="/profile" element={<Navigate to="/profiles" replace />} />
        <Route path="/u/:userId" element={<LegacyProfileRedirect />} />
        <Route path="*" element={<p className="empty-line">bash: rota não encontrada</p>} />
      </Route>
    </Route>
  </Routes>
}

function App() {
  return <BrowserRouter><AuthProvider><AppRoutes /></AuthProvider></BrowserRouter>
}

export default App