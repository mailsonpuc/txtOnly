import { Feather, House, LogOut, PenLine, UserRound, UsersRound } from 'lucide-react'
import { Link, NavLink } from 'react-router-dom'
import { UserAvatar } from './UserAvatar'

export function Sidebar({ name, onSignOut }: { name: string; onSignOut: () => void }) {
  return (
    <aside className="primary-sidebar" aria-label="Navegação principal">
      <Link className="brand-mark" to="/feed" aria-label="txtOnly, início"><span className="brand-mark-symbol"><Feather aria-hidden="true" /></span><span>txtOnly</span></Link>
      <nav className="primary-nav">
        <NavLink to="/feed"><House aria-hidden="true" /><span>Início</span></NavLink>
        <NavLink to="/users"><UsersRound aria-hidden="true" /><span>Usuários</span></NavLink>
        <NavLink to="/profiles" end><UserRound aria-hidden="true" /><span>Perfil</span></NavLink>
      </nav>
      <Link className="sidebar-compose" to="/feed#new-post"><PenLine aria-hidden="true" /><span>Publicar</span></Link>
      <div className="sidebar-account"><UserAvatar name={name} /><div><strong>{name}</strong><span>conta ativa</span></div><button className="icon-button account-exit" onClick={onSignOut} aria-label="Sair da conta" title="Sair"><LogOut aria-hidden="true" /></button></div>
      <nav className="mobile-navigation" aria-label="Navegação principal mobile">
        <NavLink to="/feed" aria-label="Início"><House aria-hidden="true" /></NavLink>
        <NavLink to="/users" aria-label="Usuários"><UsersRound aria-hidden="true" /></NavLink>
        <NavLink to="/profiles" end aria-label="Perfil"><UserRound aria-hidden="true" /></NavLink>
      </nav>
    </aside>
  )
}