// Chart.js utility functions for statistics dashboard

let chartInstances = {};

function destroyChart(chartId) {
    if (chartInstances[chartId]) {
        chartInstances[chartId].destroy();
        delete chartInstances[chartId];
    }
}

function showFallback(chartId) {
    const canvas = document.getElementById(chartId);
    const fallback = document.getElementById(chartId + 'Fallback');
    if (canvas) canvas.style.display = 'none';
    if (fallback) fallback.style.display = 'block';
}

function hideCanvasIfChartFails(chartId) {
    if (typeof Chart === 'undefined') {
        showFallback(chartId);
        return true;
    }
    return false;
}

// Utility function to safely handle null/undefined data
function safeArray(data) {
    return Array.isArray(data) ? data : [];
}

// Utility function to safely get a value with a default
function safeValue(value, defaultValue) {
    return (value === null || value === undefined) ? defaultValue : value;
}

// Utility function to safely get a string with a default
function safeString(value, defaultValue) {
    if (value === null || value === undefined || value === '') {
        return defaultValue || 'Unspecified';
    }
    return value;
}

window.renderWaitTimeChart = function(data) {
    if (hideCanvasIfChartFails('waitTimeChart')) return;
    
    destroyChart('waitTimeChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = document.getElementById('waitTimeChart').getContext('2d');
        chartInstances['waitTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.range, 'Unknown')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('waitTimeChart');
    }
};

window.renderTreatmentTimeChart = function(data) {
    if (hideCanvasIfChartFails('treatmentTimeChart')) return;
    
    destroyChart('treatmentTimeChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = document.getElementById('treatmentTimeChart').getContext('2d');
        chartInstances['treatmentTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.range, 'Unknown')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTimeChart');
    }
};

window.renderTreatmentTypesChart = function(data) {
    if (hideCanvasIfChartFails('treatmentTypesChart')) return;
    
    destroyChart('treatmentTypesChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = document.getElementById('treatmentTypesChart').getContext('2d');
        chartInstances['treatmentTypesChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.treatmentName, 'Unspecified')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.patientCount, 0)),
                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTypesChart');
    }
};

window.renderCounsellorChart = function(data) {
    if (hideCanvasIfChartFails('counsellorChart')) return;
    
    destroyChart('counsellorChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = document.getElementById('counsellorChart').getContext('2d');
        chartInstances['counsellorChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.counsellorName, 'Unspecified')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.patientCount, 0)),
                    backgroundColor: 'rgba(153, 102, 255, 0.6)',
                    borderColor: 'rgba(153, 102, 255, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('counsellorChart');
    }
};

window.renderLanguageChart = function(demographics) {
    if (hideCanvasIfChartFails('languageChart')) return;
    
    destroyChart('languageChart');
    
    try {
        // Handle null demographics object
        const safeDemographics = demographics || {};
        const ctx = document.getElementById('languageChart').getContext('2d');
        chartInstances['languageChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['English First Language', 'Other Language'],
                datasets: [{
                    data: [
                        safeValue(safeDemographics.englishFirstLanguageCount, 0), 
                        safeValue(safeDemographics.otherLanguageCount, 0)
                    ],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 206, 86, 0.8)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('languageChart');
    }
};

window.renderSurveyChart = function(demographics) {
    if (hideCanvasIfChartFails('surveyChart')) return;
    
    destroyChart('surveyChart');
    
    try {
        // Handle null demographics object
        const safeDemographics = demographics || {};
        const ctx = document.getElementById('surveyChart').getContext('2d');
        chartInstances['surveyChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Survey Returned', 'Survey Not Returned'],
                datasets: [{
                    data: [
                        safeValue(safeDemographics.surveyReturnedCount, 0), 
                        safeValue(safeDemographics.surveyNotReturnedCount, 0)
                    ],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 99, 132, 0.8)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('surveyChart');
    }
};

window.renderTrendsChart = function(data) {
    if (hideCanvasIfChartFails('trendsChart')) return;
    
    destroyChart('trendsChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = document.getElementById('trendsChart').getContext('2d');
        chartInstances['trendsChart'] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: safeData.map(d => safeString(d.period, 'Unknown')),
                datasets: [{
                    label: 'Referrals',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(255, 159, 64, 0.2)',
                    borderColor: 'rgba(255, 159, 64, 1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('trendsChart');
    }
};

window.renderCareTypesChart = function(data) {
    destroyChart('careTypesChart');
    
    if (!data || data.length === 0) return;
    
    const ctx = document.getElementById('careTypesChart').getContext('2d');
    
    // Define colors for care types - map by care type name for consistency
    const colorMap = {
        'Adjuvant': { bg: 'rgba(46, 204, 113, 0.8)', border: 'rgba(46, 204, 113, 1)' },           // Green
        'Palliative': { bg: 'rgba(155, 89, 182, 0.8)', border: 'rgba(155, 89, 182, 1)' },         // Purple
        'Adjuvant & Palliative': { bg: 'rgba(52, 152, 219, 0.8)', border: 'rgba(52, 152, 219, 1)' }, // Blue
        'Unspecified': { bg: 'rgba(149, 165, 166, 0.8)', border: 'rgba(149, 165, 166, 1)' }       // Gray
    };
    
    const backgroundColors = data.map(d => colorMap[d.careType]?.bg || 'rgba(189, 195, 199, 0.8)');
    const borderColors = data.map(d => colorMap[d.careType]?.border || 'rgba(189, 195, 199, 1)');
    
    chartInstances['careTypesChart'] = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: data.map(d => d.careType),
            datasets: [{
                data: data.map(d => d.patientCount),
                backgroundColor: backgroundColors,
                borderColor: borderColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const value = context.parsed;
                            const percentage = data[context.dataIndex].percentage.toFixed(1);
                            return `${context.label}: ${value} patients (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
};

