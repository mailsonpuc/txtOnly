import { useId, useState } from 'react'

export function ExpandableText({ text, limit = 280, lines = 5, className = '' }: {
  text: string
  limit?: number
  lines?: number
  className?: string
}) {
  const [expanded, setExpanded] = useState(false)
  const contentId = useId()
  const canExpand = text.trim().length > limit

  return (
    <div className={`expandable-text ${className}`.trim()}>
      <p id={contentId} className={canExpand && !expanded ? 'expandable-copy is-clamped' : 'expandable-copy'} style={{ '--excerpt-lines': lines } as React.CSSProperties}>{text}</p>
      {canExpand && <button className="expandable-toggle" type="button" aria-expanded={expanded} aria-controls={contentId} onClick={() => setExpanded((value) => !value)}>
        {expanded ? 'Ver menos' : 'Ver mais'}
      </button>}
    </div>
  )
}