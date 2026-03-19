import { useState, useCallback } from 'react'
import './App.css'

const API = 'https://calculatrice-api-dp8n.onrender.com'

// -- Utilitaires ───────────────────────────────────────────────────────────────
const fmt = v =>
  v === undefined || v === null ? ''
  : Number.isInteger(v) ? String(v)
  : parseFloat(v.toFixed(6)).toString()

// ── Layout SVG ───────────────────────────────────────────────────────────── CHATGPT !
const NW = 64, NH = 36, HGAP = 12, VGAP = 52

function layoutTree(node) {
  function measure(n) {
    if (n.type === 'number')   { n.w = NW; return }
    if (n.type === 'function') { measure(n.argument); n.w = Math.max(NW, n.argument.w); return }
    measure(n.left); measure(n.right)
    n.w = n.left.w + HGAP + n.right.w
  }

  function place(n, x, y) {
    if (n.type === 'number')   { n.x = x; n.y = y; return }
    if (n.type === 'function') {
      n.x = x + n.w/2 - NW/2; n.y = y
      place(n.argument, x + n.w/2 - n.argument.w/2, y + NH + VGAP)
      return
    }
    n.x = x + n.left.w + HGAP/2 - NW/2; n.y = y
    place(n.left,  x, y + NH + VGAP)
    place(n.right, x + n.left.w + HGAP, y + NH + VGAP)
  }

  measure(node)
  place(node, 16, 16)

  let maxX = 0, maxY = 0
  function bbox(n) {
    maxX = Math.max(maxX, n.x + NW + 16)
    maxY = Math.max(maxY, n.y + NH + 16)
    if (n.type === 'binairy')   { bbox(n.left); bbox(n.right) }
    if (n.type === 'function') bbox(n.argument)
  }
  bbox(node)
  return { width: maxX, height: maxY }
}

function collectNodes(n, arr = []) {
  arr.push(n)
  if (n.type === 'binairy')   { collectNodes(n.left, arr); collectNodes(n.right, arr) }
  if (n.type === 'function') collectNodes(n.argument, arr)
  return arr
}

function collectEdges(n, arr = []) {
  if (n.type === 'binairy') {
    arr.push([n, n.left], [n, n.right])
    collectEdges(n.left, arr); collectEdges(n.right, arr)
  }
  if (n.type === 'function') {
    arr.push([n, n.argument])
    collectEdges(n.argument, arr)
  }
  return arr
}

// ── Composants ───────────────────────────────────────────────────────────── 50/50 CHATGPT !
function TreeNode({ node }) {
  const cx = node.x + NW / 2
  const rectClass = node.type  === 'number' ? 'node-num'
                  : node.type  === 'binairy' ? 'node-op'
                  : 'node-fn'
  const textClass = node.type  === 'number' ? 'text-num'
                  : node.type  === 'binairy' ? 'text-op'
                  : 'text-fn'
  const subClass  = node.type  === 'number' ? 'text-num-sub'
                  : node.type  === 'binairy' ? 'text-op-sub'
                  : 'text-fn-sub'
  const label = node.type  === 'number'   ? fmt(node.value)
              : node.type  === 'binairy'   ? node.operator
              : node.function

  return (
    <>
      <rect
        x={node.x} y={node.y} width={NW} height={NH}
        rx={node.type === 'number' ? 6 : 10}
        className={rectClass} strokeWidth={1}
      />
      {node.type !== 'number' ? (
        <>
          <text x={cx} y={node.y + 13} fontSize={13} fontWeight={500}
            textAnchor="middle" dominantBaseline="central"
            className={textClass}>
            {label}
          </text>
          <text x={cx} y={node.y + 27} fontSize={11}
            textAnchor="middle" dominantBaseline="central"
            className={subClass}>
            = {fmt(node.eval)}
          </text>
        </>
      ) : (
        <text x={cx} y={node.y + NH / 2} fontSize={13} fontWeight={500}
          textAnchor="middle" dominantBaseline="central"
          className={textClass}>
          {label}
        </text>
      )}
    </>
  )
}

// -- Tokens Copy du backend
const TOKEN_CLASS = {
  Number: 'token-number',
  Plus:   'token-op',
  Minus:  'token-op',
  Multiply:   'token-op',
  Divide:  'token-op',
  Power:  'token-op',
  LeftParenthesis: 'token-paren',
  RightParenthesis: 'token-paren',
  Sqrt:   'token-fn',
  Sin:   'token-fn',
  Cos:   'token-fn',
  Tan:   'token-fn',
  Log:   'token-fn',
}

function TokenBadge({ token }) {
  const cls = TOKEN_CLASS[token.type] ?? 'token-number'
  return <span className={`token ${cls}`}>{token.value}</span>
}

// ── App ────────────────────────────────────────────────────────────────────
const EXAMPLES = [
  '1 + 1',
  '1 + 2',
  '1 + -1',
  '-1 - -1',
  '5 - 4',
  '5 * 2',
  '(2 + 5) *3',
  '10 / 2',
  '2 + 2 * 5 + 5',
  '2.8 * 3 - 1',
  '2^8',
  '2^8 * 5 - 1',
  'sqrt(4)',
  '1 / 0'
]

export default function App() {
  const [expr, setExpr]       = useState('1 + 1')
  const [result, setResult]   = useState(null)
  const [tree, setTree]       = useState(null)
  const [tokens, setTokens]   = useState([])
  const [dims, setDims]       = useState({ width: 0, height: 0 })
  const [error, setError]     = useState(null)
  const [loading, setLoading] = useState(false)

  const calculate = useCallback(async (value) => {
    const e = value ?? expr
    setLoading(true)
    setError(null)

    try {
      const res = await fetch(`${API}/evaluate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Input: e })
      })
      
      const data = await res.json()
      console.log(JSON.stringify(data.tree, null, 2))
      if (!res.ok) {
        setError(data.error)
        setTree(null); setResult(null); setTokens([])
        return
      }
      setResult(data.result)
      setTree(data.tree)
      setTokens(data.tokens)
      setDims(layoutTree(data.tree))

    } catch {
      setError('Impossible de contacter le serveur.')
    } finally {
      setLoading(false)
    }
  }, [expr])

  return (
    <div className="container">
      <h2>Évaluateur d'expressions</h2>

      {/* Input */}
      <div className="input-row">
        <input
          value={expr}
          onChange={e => setExpr(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && calculate()}
          placeholder="ex: 1 + 1"
        />
        <button onClick={() => calculate()} disabled={loading}>
          {loading ? '...' : 'Calculer'}
        </button>
      </div>

      {/* Exemples */}
      <div className="examples">
        <span>Exemples :</span>
        {EXAMPLES.map(ex => (
          <button key={ex} onClick={() => { setExpr(ex); calculate(ex) }}>{ex}</button>
        ))}
      </div>

      {/* Erreur */}
      {error && <div className="error-box">{error}</div>}

      {/* Résultat */}
      {result !== null && !error && (
        <div className="result-box">
          <span className="label">Résultat</span>
          <span className="value">{fmt(result)}</span>
        </div>
      )}

      {/* Tokens */}
      {tokens.length > 0 && (
        <div className="tokens-section">
          <p className="section-label">Tokens</p>
          <div className="tokens-list">
            {tokens.map((tok, i) => <TokenBadge key={i} token={tok} />)}
          </div>
        </div>
      )}

      {/* Arbre */}
      {tree && (
        <div className="tree-section">
          <svg
            width={dims.width} height={dims.height}
            viewBox={`0 0 ${dims.width} ${dims.height}`}
            style={{ display: 'block', margin: '0 auto' }}
          >
            {collectEdges(tree).map(([p, c], i) => (
              <line key={i}
                x1={p.x + NW/2} y1={p.y + NH}
                x2={c.x + NW/2} y2={c.y}
                className="tree-edge" strokeWidth={1}
              />
            ))}
            {collectNodes(tree).map((n, i) => (
              <TreeNode key={i} node={n} />
            ))}
          </svg>
        </div>
      )}
    </div>
  )
}