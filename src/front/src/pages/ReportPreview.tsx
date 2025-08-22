import { useRef } from 'react'
import jsPDF from 'jspdf'
import html2canvas from 'html2canvas'
import { buildReportHtml } from '../utils/csrdTemplate'

export default function ReportPreview() {
  const containerRef = useRef<HTMLDivElement>(null)

  const generate = async () => {
    const el = containerRef.current
    if (!el) return

    const canvas = await html2canvas(el, { scale: 2, useCORS: true, backgroundColor: "#ffffff" })
    const imgData = canvas.toDataURL('image/png')
    const pdf = new jsPDF('p', 'mm', 'a4')
    const pageWidth = pdf.internal.pageSize.getWidth()
    const pageHeight = pdf.internal.pageSize.getHeight()

    // Scale image to fit width and paginate if needed
    const imgWidth = pageWidth
    const imgHeight = canvas.height * (imgWidth / canvas.width)

    let y = 0
    let remaining = imgHeight
    const sliceHeightPx = canvas.width * (pageHeight / pageWidth) // proportional slice

    while (remaining > 0) {
      const pageCanvas = document.createElement('canvas')
      pageCanvas.width = canvas.width
      pageCanvas.height = Math.min(sliceHeightPx, remaining)
      const ctx = pageCanvas.getContext('2d')!
      ctx.drawImage(canvas, 0, y, canvas.width, pageCanvas.height, 0, 0, pageCanvas.width, pageCanvas.height)
      const pageImgData = pageCanvas.toDataURL('image/png')
      if (y > 0) pdf.addPage()
      pdf.addImage(pageImgData, 'PNG', 0, 0, pageWidth, pageHeight)
      y += pageCanvas.height
      remaining -= pageCanvas.height
    }

    pdf.save('Rapport_CSRT_GreenTrace.pdf')
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Rapport CSRD</h1>
        <p className="text-slate-600">Prévisualisation du rapport généré à partir de vos données. Export PDF inclus.</p>
      </div>

      <div className="card">
        <div ref={containerRef} dangerouslySetInnerHTML={{ __html: buildReportHtml() }} />
      </div>

      <button className="btn btn-primary" onClick={generate}>Exporter en PDF</button>

      <div className="text-xs text-slate-500">
        Note: modèle simplifié destiné à la démo. Pour un rapport 100% conforme, ajoutez l’intégralité des exigences ESRS (disclosures, datapoints, annexes) et un mapping réglementaire.
      </div>
    </div>
  )
}
