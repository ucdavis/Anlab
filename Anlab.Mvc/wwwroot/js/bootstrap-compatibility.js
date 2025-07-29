// Bootstrap 3 to 5 Compatibility JavaScript

(function($) {
    'use strict';

    $(document).ready(function() {
        
        // Handle legacy Bootstrap 3 data attributes
        function migrateDataAttributes() {
            // Convert data-toggle attributes
            $('[data-toggle]').each(function() {
                var $this = $(this);
                var toggle = $this.attr('data-toggle');
                
                switch(toggle) {
                    case 'tooltip':
                        $this.attr('data-bs-toggle', 'tooltip');
                        new bootstrap.Tooltip(this);
                        break;
                    case 'dropdown':
                        $this.attr('data-bs-toggle', 'dropdown');
                        break;
                    case 'collapse':
                        $this.attr('data-bs-toggle', 'collapse');
                        var target = $this.attr('data-target');
                        if (target) {
                            $this.attr('data-bs-target', target);
                        }
                        break;
                    case 'modal':
                        $this.attr('data-bs-toggle', 'modal');
                        var target = $this.attr('data-target');
                        if (target) {
                            $this.attr('data-bs-target', target);
                        }
                        break;
                }
            });

            // Convert data-dismiss attributes
            $('[data-dismiss]').each(function() {
                var $this = $(this);
                var dismiss = $this.attr('data-dismiss');
                $this.attr('data-bs-dismiss', dismiss);
            });

            // Convert data-target to data-bs-target
            $('[data-target]').each(function() {
                var $this = $(this);
                if (!$this.attr('data-bs-target')) {
                    $this.attr('data-bs-target', $this.attr('data-target'));
                }
            });

            // Convert data-parent to data-bs-parent for accordion
            $('[data-parent]').each(function() {
                var $this = $(this);
                $this.attr('data-bs-parent', $this.attr('data-parent'));
            });
        }

        // Handle Bootstrap 3 panel groups to Bootstrap 5 accordions
        function migratePanelGroups() {
            $('.panel-group').each(function() {
                var $panelGroup = $(this);
                $panelGroup.addClass('accordion');
                
                $panelGroup.find('.panel').each(function(index) {
                    var $panel = $(this);
                    var $heading = $panel.find('.panel-heading');
                    var $body = $panel.find('.panel-body');
                    var $collapse = $panel.find('.panel-collapse');
                    
                    // Generate unique ID if not present
                    var collapseId = $collapse.attr('id') || 'collapse-' + index;
                    $collapse.attr('id', collapseId);
                    
                    // Convert panel to accordion item
                    $panel.addClass('accordion-item');
                    
                    // Convert heading
                    if ($heading.length) {
                        $heading.addClass('accordion-header');
                        var $title = $heading.find('.panel-title a, .panel-title');
                        if ($title.length) {
                            var buttonHtml = '<button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#' + collapseId + '" aria-expanded="false" aria-controls="' + collapseId + '">' + $title.text() + '</button>';
                            $heading.html('<h2 class="accordion-header">' + buttonHtml + '</h2>');
                        }
                    }
                    
                    // Convert body
                    if ($body.length) {
                        $body.addClass('accordion-body');
                    }
                    
                    // Convert collapse
                    if ($collapse.length) {
                        $collapse.addClass('accordion-collapse');
                        if (!$collapse.hasClass('show') && !$collapse.hasClass('in')) {
                            $collapse.addClass('collapse');
                        }
                    }
                });
            });
        }

        // Handle legacy tooltip initialization
        function initializeLegacyTooltips() {
            // Handle tooltips that might have been initialized with Bootstrap 3 syntax
            $('[data-toggle="tooltip"], [title]').not('[data-bs-toggle]').each(function() {
                if ($(this).attr('title') || $(this).attr('data-original-title')) {
                    new bootstrap.Tooltip(this);
                }
            });
        }

        // Handle legacy modal triggers
        function handleLegacyModals() {
            $('[data-toggle="modal"]').not('[data-bs-toggle]').each(function() {
                var $this = $(this);
                $this.attr('data-bs-toggle', 'modal');
                if ($this.attr('data-target')) {
                    $this.attr('data-bs-target', $this.attr('data-target'));
                }
            });
        }

        // Handle legacy dropdown triggers
        function handleLegacyDropdowns() {
            $('.dropdown-toggle').each(function() {
                var $this = $(this);
                if (!$this.attr('data-bs-toggle')) {
                    $this.attr('data-bs-toggle', 'dropdown');
                }
            });
        }

        // Initialize all compatibility features
        migrateDataAttributes();
        migratePanelGroups();
        initializeLegacyTooltips();
        handleLegacyModals();
        handleLegacyDropdowns();

        // Legacy event handling
        $(document).on('click', '[data-dismiss="alert"]', function() {
            $(this).closest('.alert').alert('close');
        });

    });

    // Provide legacy API compatibility
    if (typeof $.fn.tooltip === 'undefined') {
        $.fn.tooltip = function(options) {
            return this.each(function() {
                new bootstrap.Tooltip(this, options);
            });
        };
    }

    if (typeof $.fn.collapse === 'undefined') {
        $.fn.collapse = function(action) {
            return this.each(function() {
                var collapse = bootstrap.Collapse.getOrCreateInstance(this);
                if (typeof action === 'string') {
                    collapse[action]();
                }
            });
        };
    }

    if (typeof $.fn.modal === 'undefined') {
        $.fn.modal = function(action) {
            return this.each(function() {
                var modal = bootstrap.Modal.getOrCreateInstance(this);
                if (typeof action === 'string') {
                    modal[action]();
                }
            });
        };
    }

})(jQuery);
