// Small client-side enhancement: confirm before submitting status-changing actions.
document.addEventListener("submit", function (event) {
    const form = event.target;
    if (form.matches('form[action*="MarkSuspicious"]')) {
        if (!window.confirm("Mark this network connection as suspicious?")) event.preventDefault();
    }
});
