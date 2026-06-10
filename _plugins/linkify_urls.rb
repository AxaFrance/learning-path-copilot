# frozen_string_literal: true

require 'jekyll'

module Jekyll
  # Linkifies raw URLs (http and https) in rendered HTML pages.
  # Pure-Ruby implementation (no external gems): it walks the HTML and only
  # touches text outside of tags and outside of protected blocks
  # (<pre>, <code>, <a>) so existing markup and code samples stay intact.
  module LinkifyUrls
    # Match http:// or https:// followed by non-whitespace URL characters.
    URL_PATTERN = %r{https?://[^\s<>"`')\]]+}

    # Partition the HTML into protected segments (group 1) and plain text
    # (group 2). Protected segments are preserved verbatim; only plain text
    # is linkified.
    SEGMENT_PATTERN = %r{
      (
        <pre\b[^>]*>.*?</pre>      |  # code blocks
        <code\b[^>]*>.*?</code>   |  # inline code
        <a\b[^>]*>.*?</a>         |  # existing links
        <[^>]+>                      # any other HTML tag
      )
      |
      ([^<]+)                          # plain text run
    }mix

    def self.process(html)
      html.gsub(SEGMENT_PATTERN) do
        protected_part = Regexp.last_match(1)
        text_part = Regexp.last_match(2)
        protected_part ? protected_part : linkify_text(text_part)
      end
    end

    def self.linkify_text(text)
      text.gsub(URL_PATTERN) do |url|
        # Keep trailing punctuation out of the link target.
        clean_url = url.sub(/[.,;:!?]+$/, '')
        trailing = url[clean_url.length..] || ''
        %(<a href="#{clean_url}" target="_blank" rel="noopener">#{clean_url}</a>#{trailing})
      end
    end
  end

  # Hook to process HTML after page rendering.
  Hooks.register :pages, :post_render do |page|
    next if page.output.nil?
    next unless page.output.is_a?(String)
    next unless page.output_ext == '.html'

    begin
      page.output = Jekyll::LinkifyUrls.process(page.output)
    rescue StandardError => e
      Jekyll.logger.warn('LinkifyUrls:', "Error processing #{page.path}: #{e.message}")
    end
  end
end
