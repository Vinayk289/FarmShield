// FarmShield – Production JavaScript Engine  v3.0

document.addEventListener('DOMContentLoaded', function () {

    // ── 1. Sidebar Toggle ──────────────────────────────────────────
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar       = document.getElementById('sidebar-wrapper');
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function (e) {
            e.preventDefault();
            sidebar.classList.toggle('toggled');
        });
        // Close sidebar on outside click (mobile)
        document.addEventListener('click', function (e) {
            if (window.innerWidth < 992 &&
                sidebar.classList.contains('toggled') &&
                !sidebar.contains(e.target) &&
                e.target !== sidebarToggle &&
                !sidebarToggle.contains(e.target)) {
                sidebar.classList.remove('toggled');
            }
        });
    }

    // ── 2. Auto-dismiss alerts after 6 s ──────────────────────────
    document.querySelectorAll('.alert-dismissible').forEach(function (el) {
        setTimeout(function () {
            const closeBtn = el.querySelector('.btn-close');
            if (closeBtn) closeBtn.click();
        }, 6000);
    });

    // ── 3. Active nav link on current page ────────────────────────
    document.querySelectorAll('.menu-link').forEach(function (link) {
        if (link.classList.contains('active')) return;
        if (link.href && link.href === window.location.href) {
            link.classList.add('active');
        }
    });

    // ── 4. Real-time Biosecurity Compliance Calculator ─────────────
    const inspectionForm = document.getElementById('inspectionForm');
    if (inspectionForm) {
        const complianceBadge    = document.getElementById('compliancePercentageBadge');
        const complianceMeter    = document.getElementById('complianceMeterBar');
        const riskLevelBadge     = document.getElementById('riskLevelBadge');
        const compliantCount     = document.getElementById('compliantCount');
        const nonCompliantCount  = document.getElementById('nonCompliantCount');
        const notApplicableCount = document.getElementById('notApplicableCount');

        function recalcCompliance() {
            const checked = document.querySelectorAll('.checklist-status-radio:checked');
            let compliant = 0, nonCompliant = 0, notApplicable = 0;
            checked.forEach(function (r) {
                const v = r.value;
                if (v === 'Compliant'    || v === '1') compliant++;
                else if (v === 'NonCompliant'  || v === '2') nonCompliant++;
                else if (v === 'NotApplicable' || v === '4') notApplicable++;
            });

            const applicable = compliant + nonCompliant;
            const pct = applicable > 0 ? Math.round((compliant / applicable) * 1000) / 10 : 100.0;

            if (compliantCount)     compliantCount.textContent     = compliant;
            if (nonCompliantCount)  nonCompliantCount.textContent  = nonCompliant;
            if (notApplicableCount) notApplicableCount.textContent = notApplicable;
            if (complianceBadge)    complianceBadge.textContent    = pct + '%';

            if (complianceMeter) {
                complianceMeter.style.width = pct + '%';
                complianceMeter.className = 'compliance-meter-bar ' +
                    (pct >= 85 ? 'bg-success' : pct >= 70 ? 'bg-warning' : 'bg-danger');
            }
            if (riskLevelBadge) {
                if (pct >= 85) {
                    riskLevelBadge.className = 'badge bg-success';
                    riskLevelBadge.textContent = 'Low Risk (Certified)';
                } else if (pct >= 70) {
                    riskLevelBadge.className = 'badge bg-warning text-dark';
                    riskLevelBadge.textContent = 'Medium Risk (Action Needed)';
                } else {
                    riskLevelBadge.className = 'badge bg-danger';
                    riskLevelBadge.textContent = 'High Bio-Hazard Risk';
                }
            }
        }

        document.querySelectorAll('.checklist-status-radio').forEach(function (r) {
            r.addEventListener('change', recalcCompliance);
        });
        recalcCompliance();
    }

    // ── 5. Tooltip initialisation ──────────────────────────────────
    if (typeof bootstrap !== 'undefined') {
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
            new bootstrap.Tooltip(el, { trigger: 'hover' });
        });
    }

}); // end DOMContentLoaded


// ══════════════════════════════════════════════════════════════════
// 6. Dashboard Charts (Chart.js 4)
// ══════════════════════════════════════════════════════════════════
window.initDashboardCharts = function (data) {
    if (!data) return;

    const PALETTE = {
        green:  '#176B45',
        greenM: 'rgba(23,107,69,0.10)',
        amber:  '#E69500',
        blue:   '#0284C7',
        red:    '#DC2626',
        orange: '#EA580C',
        teal:   '#16A34A',
        font:   '#52665A',
    };

    const baseOpts = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { labels: { font: { family: 'Inter', weight: '600', size: 12 }, boxWidth: 12 } } }
    };

    // Chart 1 – Farm type distribution (Doughnut)
    const ctxDist = document.getElementById('farmDistributionChart');
    if (ctxDist && data.farmTypeCounts) {
        new Chart(ctxDist, {
            type: 'doughnut',
            data: {
                labels: data.farmTypeLabels || ['Pig Farms','Poultry Farms','Mixed Farms'],
                datasets: [{ data: data.farmTypeCounts, backgroundColor: [PALETTE.green, PALETTE.amber, PALETTE.blue], borderWidth: 2, borderColor: '#fff' }]
            },
            options: { ...baseOpts, cutout: '68%', plugins: { legend: { position: 'bottom', labels: { font: { family: 'Inter', weight: '600', size: 12 }, boxWidth: 12 } } } }
        });
    }

    // Chart 2 – Compliance trends (Line)
    const ctxCompliance = document.getElementById('complianceTrendsChart');
    if (ctxCompliance && data.complianceTrends) {
        new Chart(ctxCompliance, {
            type: 'line',
            data: {
                labels: data.monthlyLabels || [],
                datasets: [{
                    label: 'Compliance %', data: data.complianceTrends,
                    borderColor: PALETTE.green, backgroundColor: PALETTE.greenM,
                    tension: 0.35, fill: true,
                    pointBackgroundColor: PALETTE.green, pointRadius: 5, pointHoverRadius: 7
                }]
            },
            options: { ...baseOpts, scales: { y: { min: 60, max: 100, ticks: { callback: v => v + '%' } }, x: { grid: { display: false } } }, plugins: { legend: { display: false } } }
        });
    }

    // Chart 3 – Monthly inspections (Stacked bar)
    const ctxInsp = document.getElementById('inspectionsBarChart');
    if (ctxInsp && data.monthlyInspectionsCompleted) {
        new Chart(ctxInsp, {
            type: 'bar',
            data: {
                labels: data.monthlyLabels || [],
                datasets: [
                    { label: 'Completed', data: data.monthlyInspectionsCompleted, backgroundColor: PALETTE.green, borderRadius: 6 },
                    { label: 'Pending',   data: data.monthlyInspectionsPending,   backgroundColor: PALETTE.amber, borderRadius: 6 }
                ]
            },
            options: { ...baseOpts, scales: { x: { stacked: true, grid: { display: false } }, y: { stacked: true, beginAtZero: true } }, plugins: { legend: { position: 'top', labels: { font: { family: 'Inter', weight: '600', size: 12 }, boxWidth: 12 } } } }
        });
    }

    // Chart 4 – Disease severity (Bar)
    const ctxDisease = document.getElementById('diseaseSeverityChart');
    if (ctxDisease && data.diseaseSeverityCounts) {
        new Chart(ctxDisease, {
            type: 'bar',
            data: {
                labels: data.diseaseSeverityLabels || ['Critical','High','Medium','Low'],
                datasets: [{ label: 'Cases', data: data.diseaseSeverityCounts, backgroundColor: [PALETTE.red, PALETTE.orange, PALETTE.amber, PALETTE.teal], borderRadius: 6 }]
            },
            options: { ...baseOpts, scales: { x: { grid: { display: false } }, y: { beginAtZero: true, ticks: { stepSize: 1 } } }, plugins: { legend: { display: false } } }
        });
    }

    // Chart 5 – Vaccination status (Doughnut)
    const ctxVacc = document.getElementById('vaccinationPieChart');
    if (ctxVacc) {
        new Chart(ctxVacc, {
            type: 'doughnut',
            data: {
                labels: ['Completed','Scheduled','Overdue'],
                datasets: [{ data: [data.vaccinationsCompleted, data.vaccinationsScheduled, data.vaccinationsOverdue], backgroundColor: [PALETTE.teal, PALETTE.blue, PALETTE.red], borderWidth: 2, borderColor: '#fff' }]
            },
            options: { ...baseOpts, cutout: '60%', plugins: { legend: { position: 'bottom', labels: { font: { family: 'Inter', weight: '600', size: 12 }, boxWidth: 12 } } } }
        });
    }

    // Chart 6 – Farm registrations (Bar)
    const ctxReg = document.getElementById('farmRegistrationChart');
    if (ctxReg && data.monthlyRegistrations) {
        new Chart(ctxReg, {
            type: 'bar',
            data: {
                labels: data.monthlyLabels || [],
                datasets: [{ label: 'New Farms', data: data.monthlyRegistrations, backgroundColor: PALETTE.green, borderRadius: 6 }]
            },
            options: { ...baseOpts, scales: { x: { grid: { display: false } }, y: { beginAtZero: true, ticks: { stepSize: 1 } } }, plugins: { legend: { display: false } } }
        });
    }
};
