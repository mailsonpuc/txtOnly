import { useEffect, useRef, useState } from 'react'

function App() {
  const [command, setCommand] = useState('')
  const [entries, setEntries] = useState([])
  const inputRef = useRef(null)
  const terminalRef = useRef(null)

  useEffect(() => {
    terminalRef.current?.scrollTo({ top: terminalRef.current.scrollHeight })
  }, [entries])

  function runCommand(event) {
    event.preventDefault()
    const value = command.trim()
    if (!value) return

    if (value === 'clear') {
      setEntries([])
      setCommand('')
      return
    }

    const [name, ...args] = value.split(' ')
    const responses = {
      help: 'Comandos disponíveis: clear, date, echo, help, ls, pwd, uname, whoami',
      date: new Date().toString(),
      ls: 'Desktop  Documents  Downloads  Music  Pictures  Projects  Videos',
      pwd: '/home/mailson',
      uname: 'Linux terminal 6.8.0-generic x86_64 GNU/Linux',
      whoami: 'mailson',
    }
    const output = name === 'echo'
      ? args.join(' ')
      : responses[name] ?? `bash: ${name}: comando não encontrado`

    setEntries((current) => [...current, { command: value, output }])
    setCommand('')
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-[#090b0a] p-4 text-[#d4d7d4] sm:p-8">
      <section className="flex h-[min(760px,calc(100vh-2rem))] w-full max-w-6xl flex-col overflow-hidden border border-[#303632] bg-black shadow-[0_24px_100px_rgba(0,0,0,0.7)] sm:h-[min(760px,calc(100vh-4rem))]">
        <header className="flex h-11 shrink-0 items-center justify-between border-b border-[#252a27] bg-[#171a18] px-4">
          <div className="flex items-center gap-2" aria-hidden="true">
            <span className="size-2.5 rounded-full bg-[#ff5f57]" />
            <span className="size-2.5 rounded-full bg-[#febc2e]" />
            <span className="size-2.5 rounded-full bg-[#28c840]" />
          </div>
          <p className="absolute left-1/2 -translate-x-1/2 font-mono text-xs text-[#9ba29d]">mailson@linux: ~</p>
          <span className="font-mono text-[10px] uppercase tracking-[0.16em] text-[#59615b]">bash</span>
        </header>

        <div
          ref={terminalRef}
          className="flex-1 overflow-y-auto p-5 font-mono text-[13px] leading-6 sm:p-8 sm:text-sm"
          onClick={() => inputRef.current?.focus()}
        >
          <div className="mb-6 text-[#aab1ac]">
            <p>GNU/Linux — shell interativo</p>
            <p>Digite <span className="text-[#8ec07c]">help</span> para ver os comandos disponíveis.</p>
          </div>

          {entries.map((entry, index) => (
            <div className="mb-3" key={`${index}-${entry.command}`}>
              <p>
                <nobr>
                  |----------------------------------------------------------|
                  | <span className="text-[#8ec07c]">mailson@linux</span>  |
                  | <span className="text-[#d79921]">:~$ </span>           |
                  |----------------------------------------------------------|
                </nobr>

                <span className="text-[#e5e9e5]">{entry.command}</span>
              </p>
              <p className={entry.output.startsWith('bash:') ? 'text-[#fb4934]' : 'whitespace-pre-wrap text-[#c1c7c2]'}>{entry.output}</p>
            </div>
          ))}

          <form className="flex items-center" onSubmit={runCommand}>
            <label className="shrink-0" htmlFor="terminal-command">
              <span className="text-[#8ec07c]">mailson@linux</span>
              <span className="text-[#d79921]">:~$ </span>
            </label>
            <input
              ref={inputRef}
              autoFocus
              autoComplete="off"
              className="min-w-0 flex-1 border-0 bg-transparent p-0 font-mono text-[#e5e9e5] outline-none ring-0 focus:ring-0"
              id="terminal-command"
              spellCheck="false"
              value={command}
              onChange={(event) => setCommand(event.target.value)}
              aria-label="Comando do terminal"
            />
            <span className="ml-1 h-4 w-2 animate-pulse bg-[#8ec07c]" aria-hidden="true" />
          </form>
        </div>

        <footer className="flex h-8 shrink-0 items-center justify-between border-t border-[#252a27] px-4 font-mono text-[10px] text-[#59615b]">
          <span>TTY1</span>
          <span>UTF-8</span>
        </footer>
      </section>
    </main>
  )
}

export default App
