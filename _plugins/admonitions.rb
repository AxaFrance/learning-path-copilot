# frozen_string_literal: true

# Build-time admonitions: converts Docusaurus-style fenced blocks
#
#   :::type Titre optionnel
#   contenu markdown...
#   :::
#
# into HTML that kramdown still renders (markdown="1"), BEFORE the
# markdown conversion happens. Pure compilation step — the source
# `.md` files keep the clean ::: syntax.
module Admonitions
  ICONS = {
    "note" => "ℹ️",
    "tip" => "💡",
    "info" => "ℹ️",
    "caution" => "⚠️",
    "warning" => "⚠️",
    "danger" => "⛔"
  }.freeze

  KNOWN = ICONS.keys.freeze

  module_function

  def convert(content)
    lines = content.split("\n", -1)
    out = []
    in_fence = false
    i = 0

    while i < lines.length
      line = lines[i]

      # Track fenced code blocks (``` or ~~~) so we never touch their content.
      if line =~ /^\s*(```|~~~)/
        in_fence = !in_fence
        out << line
        i += 1
        next
      end

      opening = line.match(/^:::(\w+)\s*(.*)$/)
      if !in_fence && opening && KNOWN.include?(opening[1].downcase)
        type = opening[1].downcase
        title = opening[2].strip
        title = type.capitalize if title.empty?

        body = []
        i += 1
        while i < lines.length && lines[i].strip != ":::"
          body << lines[i]
          i += 1
        end
        # i now points at the closing ::: (or end of file)

        out << %(<div class="admonition admonition-#{type}" markdown="1">)
        out << %(<p class="admonition-title">#{ICONS[type]} #{title}</p>)
        out << ""
        out.concat(body)
        out << "</div>"

        i += 1 # skip the closing :::
      else
        out << line
        i += 1
      end
    end

    out.join("\n")
  end
end

Jekyll::Hooks.register [:pages, :documents], :pre_render do |doc|
  next unless doc.respond_to?(:content) && doc.content
  next unless doc.content.include?(":::")

  doc.content = Admonitions.convert(doc.content)
end
